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

    private int _turnCountFullRound;
    private int _turnCountCurrent;
    private int _currentRound = 2;
    private int _nextRound = 2;
    private bool _updateRoundMarker;

    private int _playerTurnCount;
    private int _playerTurnCountToGetCard;


    // NOTE (Calle): This is so that the character that is removed in InitializeTurnOrder()
    // can be added at the end of the turn later.
    private Character _poppedCharacter = null;

    public CombatTurnOrder()
    {
        _charactersInTurnOrder         = new List<Character>();
        _charactersInPendingTurnOrder  = new List<Character>();
        _charactersInExecutedTurnOrder = new List<Character>();
        _charactersToDisplay           = new List<Character>();

        _currentRound = 1;
        _nextRound = 2;
        _playerTurnCount = 0;
        _playerTurnCountToGetCard = 4;
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
        _turnCountFullRound = _charactersInPendingTurnOrder.Count;
        _nextRound = 2;
        SortCharacterListByInitiative(_charactersInPendingTurnOrder);

        _activeCharacter = _charactersInPendingTurnOrder[0];
        
        _charactersInPendingTurnOrder.RemoveAt(0);

        _charactersInExecutedTurnOrder.Add(_activeCharacter);

        _charactersToDisplay.AddRange(_charactersInPendingTurnOrder);
        _charactersToDisplay.AddRange(_charactersInExecutedTurnOrder);

        _turnCountCurrent = _charactersInExecutedTurnOrder.Count;
        UpdateCurrentTurnType();
        GetFullRoundMarkerPosition();

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay, _nextRound);
    }
    
    public void UpdateTurnOrder()
    {
        if (_updateRoundMarker)
        {
            _currentRound++;
            _nextRound++;
            _updateRoundMarker = false;
        }
            

        if (_charactersInPendingTurnOrder.Count <= 0)
        {
            CombatEventManager.InvokeOnRoundFinished(_nextRound);
            _charactersInPendingTurnOrder.AddRange(_charactersInExecutedTurnOrder);
            _charactersInExecutedTurnOrder.Clear();
        }

        if (_charactersInPendingTurnOrder.Count == 0)
        {
            Debug.LogError("No characters available for turn order!");
            return;
        }

        _activeCharacter = _charactersInPendingTurnOrder[0];
        
        _charactersInPendingTurnOrder.RemoveAt(0);
        
        _charactersInExecutedTurnOrder.Add(_activeCharacter);
        
        _charactersToDisplay.Remove(_activeCharacter);
        _charactersToDisplay.Add(_activeCharacter);


        _turnCountCurrent = _charactersInExecutedTurnOrder.Count;
    

        UpdateCurrentTurnType();

        if (_charactersInPendingTurnOrder.Count <= 0)
            _updateRoundMarker = true;
        
        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay, _nextRound);
    }

    private void RebuildTurnOrder()
    {
        SortCharacterListByInitiative(_charactersInPendingTurnOrder);
        SortCharacterListByInitiative(_charactersInExecutedTurnOrder);

        _charactersToDisplay.Clear();
        _charactersToDisplay.AddRange(_charactersInPendingTurnOrder);
        _charactersToDisplay.AddRange(_charactersInExecutedTurnOrder);

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay, _nextRound);
    }

    private void SortCharacterListByInitiative(List<Character> list)
    {
        if (list.Count == 0)
            return;
        // Sort them byt initiative, highest first
        list.Sort((a, b) => b.GetBaseInitiative().CompareTo(a.GetBaseInitiative()));
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

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder, _nextRound);
    }

    private void HandleCharacterDeath(Character character)
    {
        _charactersInPendingTurnOrder.Remove(character);
        _charactersInExecutedTurnOrder.Remove(character);
        _charactersToDisplay.Remove(character);

        _charactersInTurnOrder.Remove(character);
        _turnCountFullRound--;
        //CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
        CombatEventManager.InvokeOnTurnOrderChanged(_charactersToDisplay, _nextRound);
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
    public int GetFullRoundMarkerPosition()
    {
        return _turnCountFullRound - _turnCountCurrent;
    }
    public int GetCurrentRound() { return _currentRound; }
    public int GetNextRound() { return _nextRound; }
    public List<Character> GetCharactersInTurnOrder()
    {
        return _charactersInTurnOrder;
    }

    public bool IsFriendlyInPendingOrder()
    {
        foreach(Character c in _charactersInPendingTurnOrder)
        {
            if (c.GetFaction() == Faction.Friendly)
                return true;
        }
        return false;
    }
    public bool IsNextRoundGetCard()
    {
        bool result = ((_currentRound + 1) % 4) == 0;
        return result;
    }
    public bool IsGetCardRound()
    {
        bool result = (_currentRound % 4) == 0;
        return result;
    }

    public void UpdateCurrentTurnType()
    {
        if (_activeCharacter.GetFaction() == Faction.Friendly)
        {
            SetCurrentTurn(CombatTurn.PlayerTurn);
            _playerTurnCount++;
            if(_playerTurnCount == _playerTurnCountToGetCard)
            {
                _playerTurnCount = 0;
            }
        }
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);
    }
}
