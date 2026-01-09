using UnityEngine;


[System.Serializable]
public class CombatStateLoadLevel : CombatStateBase
{
    public override CombatState _state => CombatState.LoadCombatLevel;

    public override void Enter()
    {
        base.Enter();
        CombatGrid._instance.LoadNextLevel();
        CombatManager._instance.InitializeCharacterDataDict();
        CombatTurnOrder combatTurnOrder = CombatManager._instance.GetCombatTurnOrder();
        //combatTurnOrder.InitializeTurnOrder();
        combatTurnOrder.Initialize();
        CombatEventManager.InvokeEnterCombatStateLoadNextLevel();

    }

    public override void Exit()
    {
        base.Exit();
        CombatEventManager.InvokeExitCombatStateLoadNextLevel();
    }

    public override void Update()
    {
        if(CombatGrid._instance.IsCombatGridLoaded())
        {
            CombatManager._instance.ChangeCombatState(new CombatStateIntroCinematic(CombatManager._instance.GetCombatCamera()));
        }
    }
}
