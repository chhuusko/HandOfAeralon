using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CombatTurn
{
    PlayerTurn,
    EnemyTurn
};

[System.Serializable]
public class CombatTurnOrder
{
    [SerializeField] private CombatTurn _currentTurn;
    [SerializeField] private Character _activeCharacter;
    [SerializeField] private List<Character> _charactersInTurnOrder;
    [SerializeField] private int _turnCountFullRound;
    [SerializeField] private int _turnCountCurrent;

    public CombatTurnOrder()
    {
        _charactersInTurnOrder = new List<Character>();
    }

    private void HandleEndCombat(bool playerWon)
    {
        CombatEventManager.OnCharacterDeath -= HandleCharacterDeath;
        CombatEventManager.OnExitCombatStateEndCombat -= HandleEndCombat;
    }

    public void InitializeTurnOrder()
    {
        // NOTE (Calle): Can't be subscribed to in constructor since it's persistant across combats, as it is
        // an instance in the CombatManager.
        CombatEventManager.OnCharacterDeath += HandleCharacterDeath;
        CombatEventManager.OnExitCombatStateEndCombat += HandleEndCombat;

        // Get all active characters in combat scene
        _charactersInTurnOrder = CombatGrid._instance.GetAllCharacterScripts();

        _turnCountCurrent       = 0;
        _turnCountFullRound     = _charactersInTurnOrder.Count;

        // Sort them byt initiative, highest first
        _charactersInTurnOrder.Sort((a,b) => b.GetInitiative().CompareTo(a.GetInitiative()));

        // T
        _activeCharacter = _charactersInTurnOrder[0];

        // Take the next character in turn off of the list
        //_charactersInTurnOrder.RemoveAt(0);

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
    }

    public void RebuildTurnOrder()
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

    public void UpdateCharacterTurnOrder()
    {
        Character nextCharacter = _charactersInTurnOrder[0];
        
        // Take the next character in turn off of the list
        _charactersInTurnOrder.RemoveAt(0);

        // Put it back in on the end of the list
        _charactersInTurnOrder.Add(nextCharacter);

        _turnCountCurrent++;

        if(nextCharacter.GetFaction() == Faction.Friendly)
            SetCurrentTurn(CombatTurn.PlayerTurn);
        else
            SetCurrentTurn(CombatTurn.EnemyTurn);

        _activeCharacter = nextCharacter;

        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
    }

    private void HandleCharacterDeath(Character character)
    {
        _charactersInTurnOrder.Remove(character);
        _turnCountFullRound--;
        CombatEventManager.InvokeOnTurnOrderChanged(_charactersInTurnOrder);
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
}
