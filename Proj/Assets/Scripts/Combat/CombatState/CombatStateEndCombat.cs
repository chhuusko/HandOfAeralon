using UnityEngine;

public class CombatStateEndCombat : CombatStateBase
{
    public override CombatState _state => CombatState.EndCombat;

    public override void Enter()
    {
        base.Enter();
        CombatEventManager.InvokeEnterCombatStateEndCombat();
    }

    public override void Exit()
    {
        base.Exit();
        CombatEventManager.InvokeExitCombatStateEndCombat();
    }

    public override void Update()
    {
        LevelManager.GetInstance().StartNextLevel();
    }
}
