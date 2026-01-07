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

        EnemyAI enemyAI = CombatManager._instance.GetEnemyAI();
        if (enemyAI)
        {
            enemyAI.AIEndTurn.AddListener(EndTurn);
        }


        CombatTurnOrder combatTurnOrder = CombatManager._instance.GetCombatTurnOrder();
        
        //NOTE (Calle): We only update and brodcast turntype event changed first turn, since it's update in EndTurn state afterwards.
        if(combatTurnOrder.IsFirstRound())
            combatTurnOrder.UpdateCurrentTurnType();

        Character activeCharacter = combatTurnOrder.GetActiveCharacter();

        activeCharacter.CanMove = true;
        
        //NOTE (Calle): Only make it possible to press "End Turn" button if its a hero
        if(activeCharacter.GetFaction() == Faction.Friendly)
        {
            CombatUI.Instance.OnEndTurnButtonPressed += EndTurn;
            CombatEventManager.OnCharacterDeath += EndTurnOnActiveCharacterDeath;
        }

        
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

        //NOTE (Calle): If character is standing still on a debuff-tile, apply corresponding status effect.
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
        {
            CombatUI.Instance.OnEndTurnButtonPressed -= EndTurn;
            CombatEventManager.OnCharacterDeath -= EndTurnOnActiveCharacterDeath;
            Selector._instance.DeselectCharacter();
        }

        CombatManager._instance.GetEnemyAI().AIEndTurn.RemoveListener(EndTurn);
    }

    public override void Update()
    {
        HandleWinCondition();
    }

    private void EndTurn()
    {
        //Debug.LogError("SOMEONE ENDED A TURN!");
        CombatManager._instance.ChangeCombatState(new CombatStateEndTurn());
    }

    private void EndTurnOnActiveCharacterDeath(Character deadCharacter)
    {
        if(deadCharacter == CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter())
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

}
