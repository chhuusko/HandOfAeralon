using UnityEngine;

public class CombatStateIntroCinematic : CombatStateBase
{
    public override CombatState _state => CombatState.IntroCinematic;

    [SerializeField] private CombatCamera _combatCamera;

    public CombatStateIntroCinematic(CombatCamera combatCamera)
    {
        _combatCamera = combatCamera;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base .Exit();
        
    }

    public override void Update()
    {
        if (_combatCamera.IsIntroCinematicDone())
            CombatManager._instance.ChangeCombatState(new CombatStateCharacterPlacement(CombatManager._instance.GetCombatSelector()));
        else
            _combatCamera.PlayIntroCinematic();
    }
}
