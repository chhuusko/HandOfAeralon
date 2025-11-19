using System;
using UnityEngine;
using UnityEngine.Events;

public class CombatStateTakeTurn : CombatStateBase
{
    public override CombatState _state => CombatState.TakeTurn;

    [SerializeField] private GameObject _activeCharacter;
    [SerializeField] private CombatTurn _currentTurn;
    [SerializeField] private PlayerTurnMode _currentPlayerTurnMode;
    [SerializeField] private GameObject _selectorOverHead;
    public UnityEvent TurnStart = new();

    public CombatStateTakeTurn(GameObject activeCharacter)
    {
        _activeCharacter = activeCharacter;
    }

    public override void Enter()
    {
        base.Enter();
        CombatEventManager.InvokeEnterCombatStateTakeTurn();
        CombatUI.Instance.OnEndTurnButtonPressed += EndTurn;

        // NOTE (Calle): Set current turn based on initiative and Faction
        _activeCharacter = GetNextTurnCharacter();
        if (_activeCharacter.GetComponent<Character>().GetFaction() == Faction.Friendly)
        {
            SetCurrentTurn(CombatTurn.PlayerTurn);
            CardHandManager._instance.ChangeMana(1);

            Vector3 position = _activeCharacter.transform.position;
            position += Vector3.up * 3.0f;
            CombatManager._instance.SetSelectorOverHeadPosition(position);
            
        }
        else if (_activeCharacter.GetComponent<Character>().GetFaction() == Faction.Enemy)
        {
            SetCurrentTurn(CombatTurn.EnemyTurn);
            CombatManager._instance.HideSelectorOverhead();
        }
    }

    public override void Exit()
    {
        base.Exit();
        CombatEventManager.InvokeExitCombatStateTakeTurn();
        CombatUI.Instance.OnEndTurnButtonPressed -= EndTurn;
    }

    public override void Update()
    {
        switch (_currentTurn)
        {
            case CombatTurn.PlayerTurn:
                HandlePlayerTurn();
                break;
            case CombatTurn.EnemyTurn:
                HandleEnemyTurn();
                break;
        }
    }

    public GameObject GetActiveCharacter()
    {
        return _activeCharacter;
    }

    private void EndTurn()
    {
        CombatManager._instance.ChangeCombatState(new CombatStateEndTurn());
    }

    private void SetCurrentTurn(CombatTurn turn)
    {
        _currentTurn = turn;
        CombatEventManager.InvokeCombatTurnChanged(turn);
    }

    public GameObject GetNextTurnCharacter()
    {
        int highestInitiative = Int32.MinValue;
        GameObject nextCharacter = null;
        foreach (var g in CombatGrid._instance.GetAllCharacters())
        {
            int initiative = g.GetComponent<Character>().GetSpeed();
            if (initiative > highestInitiative)
            {
                highestInitiative = initiative;
                nextCharacter = g;
            }
        }

        return nextCharacter;
    }

    private void HandlePlayerTurn()
    {
        // TODO (Calle): 
        //  Vid starten av varje hero karakt�rs turn sker dessa saker: 
        //  - Spelarens mana �kar med 1 -> I CardHandManager()
        //  - Hero karakt�rens ability cooldowns minskar med 1 -> WIP (MG/JOPPA)
        //  - Spelarens "cooldown" / timer f�r att dra ett till kort minskar med 1 -> WIP 

        // TODO: Call selector with character.

        //TurnStart.Invoke(); // Säger till AI att en ny tur börjat, Eventet broadcastas både här och i HandleEnemyTurn() för att AI ska kunna spela båda factions.
        switch (_currentPlayerTurnMode)
        {
            case PlayerTurnMode.CharacterMode:
                break;
            case PlayerTurnMode.CardMode:
                break;
        }

        CombatManager._instance.UpdateSelectorOverHeadPosition();
    }

    bool enemyDoingStuff = false;
    private void HandleEnemyTurn()
    {
        if (!enemyDoingStuff)
        {
            enemyDoingStuff = true;
            TurnStart.Invoke(); // Säger till AI att en ny tur börjat, Eventet broadcastas både här och i HandlePlayerTurn() för att AI ska kunna spela båda factions.
            //_activeCharacter = null;
            //UpdateCombatState(CombatState.EndTurn);
        }
    }
}
