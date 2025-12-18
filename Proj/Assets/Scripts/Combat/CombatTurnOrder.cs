using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CombatTurn
{
    PlayerTurn,
    EnemyTurn
};


/*
 * 
 * - PendingList:   Characters that have not yet done their turn
 * - ExecutedList:  Characters that have done their turn
 * - FullList:      All Characters composed of [PendingList, ExecutedList]
 * 
 *  1. Initialize the turnorder based on initiative
 *  
 *     P: 6    5   4   4   3   2   1|
 *  
 *  2.  PreTurn: Set Active to be first in the PendingList and move Active to ExecutedList from PendingList
 *    
 *    A: (6)
 *    P: 5   4   4   3   2   1|  
 *    E: 6
 * 
 *  3. ReBuild FulltList:
 *    
 *    A: (6)    
 *    P: 5   4   4   3   2   1|  
 *    E: 6
 *    F: 5   4   4   3   2   1|  6
 *  
 *  4. PostTurn: Move first in PendingList to first in ExecutedList
 *     and rebuild the FullList
 * 
 * 
 * 
 */

[System.Serializable]
public class CombatTurnOrder
{
    [SerializeField] private CombatTurn _currentTurn;
    [SerializeField] private Character _activeCharacter;

    [SerializeField] private List<Character> _charactersInTurnOrder;
    [SerializeField] private List<Character> _charactersInPendingTurnOrder;
    [SerializeField] private List<Character> _charactersInExecutedTurnOrder;
    [SerializeField] private List<Character> _charactersToDisplay;

    [SerializeField] private int _turnCountFullRound;
    [SerializeField] private int _turnCountCurrent;
    
    // NOTE (Calle): This is so that the character that is removed in InitializeTurnOrder()
    // can be added at the end of the turn later.

    private Character _poppedCharacter = null;
    public CombatTurnOrder()
    {
        _charactersInTurnOrder         = new List<Character>();
        _charactersInPendingTurnOrder  = new List<Character>();
        _charactersInExecutedTurnOrder = new List<Character>();
        _charactersToDisplay           = new List<Character>();
    }

    private void HandleEndCombat(bool playerWon)
    {
        CombatEventManager.OnCharacterDeath             -= HandleCharacterDeath;
        CombatEventManager.OnExitCombatStateEndCombat   -= HandleEndCombat;
        CombatEventManager.OnCharacterInitiativeChanged -= RebuildTurnOrder;
    }

    public void Initialize()
    {
        CombatEventManager.OnCharacterDeath           += HandleCharacterDeath;
        CombatEventManager.OnExitCombatStateEndCombat += HandleEndCombat;
        CombatEventManager.OnCharacterInitiativeChanged += RebuildTurnOrder;

        _charactersInPendingTurnOrder = CombatGrid._instance.GetAllCharacterScripts();

        SortCharacterListByInitiative(_charactersInPendingTurnOrder);

        _activeCharacter = _charactersInPendingTurnOrder[0];
        
        _charactersInPendingTurnOrder.RemoveAt(0);

        _charactersInExecutedTurnOrder.Add(_activeCharacter);

        _charactersToDisplay.AddRange(_charactersInPendingTurnOrder);
        _charactersToDisplay.AddRange(_charactersInExecutedTurnOrder);
        _turnCountCurrent++;

        UpdateCurrentTurnType();
        

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay);
    }
    
    public void UpdateTurnOrder()
    {
        _activeCharacter = _charactersInPendingTurnOrder[0];
        
        _charactersInPendingTurnOrder.RemoveAt(0);

        _charactersInExecutedTurnOrder.Add(_activeCharacter);
        _turnCountCurrent++;

        _charactersToDisplay.Remove(_activeCharacter);
        _charactersToDisplay.Add(_activeCharacter);

        if (_charactersInPendingTurnOrder.Count <= 0)
        {
            _charactersInPendingTurnOrder.AddRange(_charactersInExecutedTurnOrder);
            _charactersInExecutedTurnOrder.Clear();
        }

        UpdateCurrentTurnType();

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay);
    }

    private void RebuildTurnOrder()
    {
        SortCharacterListByInitiative(_charactersInPendingTurnOrder);
        SortCharacterListByInitiative(_charactersInExecutedTurnOrder);

        _charactersToDisplay.Clear();
        _charactersToDisplay.AddRange(_charactersInPendingTurnOrder);
        _charactersToDisplay.AddRange(_charactersInExecutedTurnOrder);

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay);
    }

    private void SortCharacterListByInitiative(List<Character> list)
    {
        // Sort them byt initiative, highest first
        list.Sort((a, b) => b.GetBaseInitiative().CompareTo(a.GetBaseInitiative()));
    }
    public void InitializeTurnOrder()
    {
        /*
        // NOTE (Calle): Can't be subscribed to in constructor since it's persistant across combats, as it is
        // an instance in the CombatManager.
        CombatEventManager.OnCharacterDeath           += HandleCharacterDeath;
        CombatEventManager.OnExitCombatStateEndCombat += HandleEndCombat;

        // Get all active characters in combat scene
        _charactersInTurnOrder = CombatGrid._instance.GetAllCharacterScripts();

        _turnCountCurrent       = 0;
        _turnCountFullRound     = _charactersInTurnOrder.Count;

        // Sort them byt initiative, highest first
        _charactersInTurnOrder.Sort((a,b) => b.GetInitiative().CompareTo(a.GetInitiative()));

        _activeCharacter = _charactersInTurnOrder[0];
        _poppedCharacter = _charactersInTurnOrder[0];

        if (_activeCharacter.GetFaction() == Faction.Friendly)
            SetCurrentTurn(CombatTurn.PlayerTurn);
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);

        // Take the first character in turn off of the list
        _charactersInTurnOrder.RemoveAt(0);

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
    */
    }

    // NOTE (Calle): This should sort the pending characters
    public void RebuildPendingTurnOrder()
    {
        // If the turn order is change during a turn (eg. from an ability that modifies the characters initiative, the
        // turn order must be rebuilt.
        _charactersInTurnOrder = CombatGrid._instance.GetAllCharacterScripts();
        _charactersInTurnOrder.Sort((a, b) => b.GetInitiative().CompareTo(a.GetInitiative()));
        
        // NOTE (Calle): If the active character is first in order after the rebuild it must be taken off
        // the list and put at the end, otherwise it will do its turn again after it's done.
        if(_activeCharacter == _charactersInTurnOrder[0])
        {
            _charactersInTurnOrder.RemoveAt(0);

            _charactersInTurnOrder.Add(_activeCharacter);
        }

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
    }

    public void UpdateCharacterTurnOrderPreTurn()
    {
        /*
        if (GetCurrentTurnCount() == 0)
        {
            if (_poppedCharacter.GetFaction() == Faction.Friendly)
                SetCurrentTurn(CombatTurn.PlayerTurn);
            else
                SetCurrentTurn(CombatTurn.EnemyTurn);
            CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
            return;
        }
            
        Character nextCharacter = _charactersInTurnOrder[0];

        _poppedCharacter = nextCharacter;        

        // Take the popped character off of the list
        _charactersInTurnOrder.RemoveAt(0);


        if(nextCharacter.GetFaction() == Faction.Friendly)
            SetCurrentTurn(CombatTurn.PlayerTurn);
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);

        _activeCharacter = nextCharacter;

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
        */
    }

    public void UpdateCharacterTurnOrderPostTurn()
    {
        // Add the popped character to the list
        if (_poppedCharacter != null)
            _charactersInTurnOrder.Add(_poppedCharacter);


        _turnCountCurrent++;
    }

    private void HandleCharacterDeath(Character character)
    {
        _charactersInPendingTurnOrder.Remove(character);
        _charactersInExecutedTurnOrder.Remove(character);
        _charactersToDisplay.Remove(character);

        _charactersInTurnOrder.Remove(character);
        _turnCountFullRound--;
        //CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay);
    }

    public void SetCurrentTurn(CombatTurn nextTurn)
    {
        _currentTurn = nextTurn;
        CombatEventManager.InvokeCombatTurnChanged(_currentTurn);
    }

    public Character GetActiveCharacter()
    {
        return _activeCharacter;
    }

    public CombatTurn GetCurrentTurn()
    {
        return _currentTurn;
    }

    public int GetCurrentTurnCount()
    {
        return _turnCountCurrent;
    }

    public int GetTurnCountFullRound()
    {
        return _turnCountFullRound;
    }

    public List<Character> GetCharactersInTurnOrder()
    {
        return _charactersInTurnOrder;
    }

    public void UpdateCurrentTurnType()
    {
        if (_activeCharacter.GetFaction() == Faction.Friendly)
            SetCurrentTurn(CombatTurn.PlayerTurn);
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);
    }
}
