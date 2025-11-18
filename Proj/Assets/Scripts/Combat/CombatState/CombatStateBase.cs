using UnityEngine;

public abstract class CombatStateBase : ICombatState
{
    public abstract CombatState _state {  get; }

    public virtual void Enter()
    {
        DebugLog.CJLog("Entering: " +  _state.ToString());
    }

    public virtual void Exit()
    {
        DebugLog.CJLog("Exiting: " + _state.ToString());
    }

    public abstract void Update(); 
}
