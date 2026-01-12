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
    [SerializeField] private Character _prevActiveCharacter;

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

    private bool _combatStarted;

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
        _combatStarted = false;
        SortCharacterListByInitiative(_charactersInPendingTurnOrder);

        SetActiveCharacter(_charactersInPendingTurnOrder[0]);
        
        _charactersInPendingTurnOrder.RemoveAt(0);

        _charactersInExecutedTurnOrder.Add(_activeCharacter);

        _charactersToDisplay.AddRange(_charactersInPendingTurnOrder);
        _charactersToDisplay.AddRange(_charactersInExecutedTurnOrder);

        _turnCountCurrent = _charactersInExecutedTurnOrder.Count;

        if (_activeCharacter.GetFaction() == Faction.Friendly)
        {
            SetCurrentTurn(CombatTurn.PlayerTurn);
        
        }
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);
        
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

        SetActiveCharacter(_charactersInPendingTurnOrder[0]);

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

    private void SetDrawCardMarker()
    {
        if(_activeCharacter)
        {
            if(_activeCharacter.GetFaction() == Faction.Enemy)
            {
                foreach(Character c in _charactersToDisplay)
                {

                }
            }
        }
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

    private static bool FriendlyWinsTie()
    {
        // 75% chance friendly wins
        return Random.value < 0.75f;
    }

    private void SortCharacterListByInitiative(List<Character> list)
    {
        if (list.Count == 0)
            return;
        // Sort them byt initiative, highest first
//        list.Sort((a, b) => b.GetBaseInitiative().CompareTo(a.GetBaseInitiative()));

        list.Sort((a, b) =>
        {
            int initiativeCompare = b.GetBaseInitiative()
                                     .CompareTo(a.GetBaseInitiative());

            // Normal initiative ordering
            if (initiativeCompare != 0)
                return initiativeCompare;

            // Equal initiative - apply 75/25 bias
            bool aFriendly = a.GetFaction() == Faction.Friendly;
            bool bFriendly = b.GetFaction() == Faction.Friendly;

            // Same faction - keep stable order
            if (aFriendly == bFriendly)
                return 0;

            // One friendly, one enemy - biased roll
            bool friendlyFirst = FriendlyWinsTie();

            return (aFriendly == friendlyFirst) ? -1 : 1;
        });
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
        if(_prevActiveCharacter != null)
        {
            if (_prevActiveCharacter.GetFaction() == _activeCharacter.GetFaction())
                return;
        }

        _currentTurn = nextTurn;
        CombatEventManager.InvokeCombatTurnChanged(_currentTurn);
    }

    public Character GetActiveCharacter()
    {
        return _activeCharacter;
    }
    public Character GetPrevActiveCharacter()
    {
        return _prevActiveCharacter;
    }
    public CombatTurn GetCurrentTurn()
    {
        return _currentTurn;
    }

    public int GetCurrentTurnCount()
    {
        return _turnCountCurrent;
    }
    public int GetPlayerTurnCount()
    {
        return _playerTurnCount;
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

    private int NumFriendlysInDisplayList()
    {
        int result = 0;
        foreach (Character c in _charactersToDisplay)
        {
            if (c.GetFaction() == Faction.Friendly)
            {
                if (result++ == 4)
                    return result;
            }
        }

        return result;
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

    public bool IsFriendlyInExecutedOrder()
    {
        foreach (Character c in _charactersInExecutedTurnOrder)
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
    public bool IsNextFriendlyTurnDrawCard()
    {
        return ((_playerTurnCount + 1) % 4) == 0;
    }

    public void UpdateCurrentTurnType()
    {
        if(_prevActiveCharacter)
        {
            if (_prevActiveCharacter.GetFaction() == Faction.Friendly)
            {
               
            }
        }

        if (_activeCharacter.GetFaction() == Faction.Friendly)
        {
            _playerTurnCount++;
            SetCurrentTurn(CombatTurn.PlayerTurn);
        }
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);
    }

    private void SetActiveCharacter(Character newActiveCharacter)
    {
        _prevActiveCharacter = _activeCharacter;
        _activeCharacter = newActiveCharacter;
    }

    public bool IsFirstRound()
    {
        if(_combatStarted)
        {
            return false;
        }
        _combatStarted = true;

        return true;
    }

}
