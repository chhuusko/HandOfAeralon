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
        
    }

    public override void Update()
    {
        CombatEventManager.InvokeExitCombatStateEndCombat();
    }
}
