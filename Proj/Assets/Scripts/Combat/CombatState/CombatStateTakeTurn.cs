using System;
using UnityEngine;
using UnityEngine.Events;

public class CombatStateTakeTurn : CombatStateBase
{
    public override CombatState _state => CombatState.TakeTurn;

    [SerializeField] private PlayerTurnMode _currentPlayerTurnMode;
    [SerializeField] private GameObject _selectorOverHead;
    public UnityEvent TurnStart = new();

    public CombatStateTakeTurn()
    {
    }

    public override void Enter()
    {
        base.Enter();   
        
        CombatUI.Instance.OnEndTurnButtonPressed += EndTurn;

        CombatTurnOrder combatTurnOrder = CombatManager._instance.GetCombatTurnOrder();

        combatTurnOrder.UpdateCharacterTurnOrder();
        Character activeCharacter = combatTurnOrder.GetActiveCharacter();

        switch(activeCharacter.GetFaction())
        {
            case Faction.Friendly:
                {
                    CardHandManager.GetInstance().ChangeMana(1);
                    
                    
                    CombatManager._instance.SetSelectorOverHeadColor(Color.green);
                }
                break;
            case Faction.Enemy:
                {
                    CombatManager._instance.SetSelectorOverHeadColor(Color.red);

                }
                break;
        }

        Vector3 selectorOverHeadPosition = activeCharacter.transform.position + (Vector3.up * 3.0f);
        CombatManager._instance.SetSelectorOverHeadPosition(selectorOverHeadPosition);

        CombatEventManager.InvokeEnterCombatStateTakeTurn(activeCharacter);
    }

    public override void Exit()
    {
        base.Exit();
        CombatEventManager.InvokeExitCombatStateTakeTurn();
        CombatUI.Instance.OnEndTurnButtonPressed -= EndTurn;
    }

    public override void Update()
    {
        switch (CombatManager._instance.GetCombatTurnOrder().GetCurrentTurn())
        {
            case CombatTurn.PlayerTurn:
                HandlePlayerTurn();
                break;
            case CombatTurn.EnemyTurn:
                HandleEnemyTurn();
                break;
        }
    }

    private void EndTurn()
    {
        CombatManager._instance.ChangeCombatState(new CombatStateEndTurn());
    }

    public GameObject GetNextTurnCharacter()
    {
        int highestInitiative = Int32.MinValue;
        GameObject nextCharacter = null;
        foreach (var g in CombatGrid._instance.GetAllCharacters())
        {
            int initiative = g.GetComponent<Character>().GetInitiative();
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
