using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class CombatStateTakeTurn : CombatStateBase
{
    public override CombatState _state => CombatState.TakeTurn;

    [SerializeField] private PlayerTurnMode _currentPlayerTurnMode;
    public UnityEvent TurnStart = new();

    public CombatStateTakeTurn()
    {
    }

    public override void Enter()
    {
        base.Enter();

        // TODO (Calle): Should AIEndTurn be in CombatEventManager, and/or should it be a event Action instead of UnityEvent?
        EnemyAI enemyAI = CombatManager._instance.GetEnemyAI();
        if(enemyAI)
        {
            enemyAI.AIEndTurn.AddListener(EndTurn);
        }
        
        
        CombatTurnOrder combatTurnOrder = CombatManager._instance.GetCombatTurnOrder();
        
        combatTurnOrder.UpdateCharacterTurnOrderPreTurn();
        Character activeCharacter = combatTurnOrder.GetActiveCharacter();
        
        //NOTE (Calle): Only make it possible to press "End Turn" button if its a hero
        if(activeCharacter.GetFaction() == Faction.Friendly)
            CombatUI.Instance.OnEndTurnButtonPressed += EndTurn;

        switch (activeCharacter.GetFaction())
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

        switch (activeCharacter.GetCurrentTileComponent().GetTileType())
        {
            case TileType.Poison:
                {
                    StatusEffectManager statusEffectManager = activeCharacter.GetComponent<StatusEffectManager>();
                    statusEffectManager.AddStatusEffect(new Poison(3));
                }
                break;
            case TileType.Lava:
                {
                    StatusEffectManager statusEffectManager = activeCharacter.GetComponent<StatusEffectManager>();
                    statusEffectManager.AddStatusEffect(new Burn(activeCharacter, 1));
                }
                break;
        }

        foreach (Character character in CombatGrid._instance.GetAllCharacterScripts())
        {
            character.ResetCurrentMovementPoints();
        }
        

        CombatEventManager.InvokeEnterCombatStateTakeTurn(activeCharacter);
    }

    public override void Exit()
    {
        base.Exit();
        CombatEventManager.InvokeExitCombatStateTakeTurn();

        if (CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter().GetFaction() == Faction.Friendly)
            CombatUI.Instance.OnEndTurnButtonPressed -= EndTurn;

        CombatManager._instance.GetEnemyAI().AIEndTurn.RemoveListener(EndTurn);
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

        HandleWinCondition();
    }

    private void EndTurn()
    {
        CombatManager._instance.ChangeCombatState(new CombatStateEndTurn());
    }

    private void HandleWinCondition()
    {
        if (CombatGrid._instance.GetAllEnemyCharacters().Count == 0)
        {
            // TODO (Calle): All enemies killed, Go directly to EndCombat State.
            CombatManager._instance.ChangeCombatState(new CombatStateEndCombat(true));
        }
        else if(CombatGrid._instance.GetAllFriendlyCharacters().Count == 0)
        {
            // TODO (Calle): All heroes killed, Go directly to EndCombat State.
            CombatManager._instance.ChangeCombatState(new CombatStateEndCombat(false));
        }
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
    }

    bool enemyDoingStuff = false;
    private void HandleEnemyTurn()
    {
        if (!enemyDoingStuff)
        {
            enemyDoingStuff = true;
             // Säger till AI att en ny tur börjat, Eventet broadcastas både här och i HandlePlayerTurn() för att AI ska kunna spela båda factions.
            //_activeCharacter = null;
            //UpdateCombatState(CombatState.EndTurn);
        }
    }
}
