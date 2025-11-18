using UnityEngine;

public class CombatStateEndTurn : CombatStateBase
{
    public override CombatState _state => CombatState.EndTurn;

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        // TODO (Calle): If all characters are dead, either friendly or enemies or Quit Game?
        //               transition to EndCombat State.
        //               If not, transition to TakeTurn again

        // NOTE (Calle): Check End Combat conditions
        if (CombatGrid._instance.GetAllEnemyCharacters().Count == 0)
        {
            // TODO (Calle): All enemies killed, do something specific to that.
            CombatManager._instance.ChangeCombatState(new CombatStateEndCombat());
        }
        else if (CombatGrid._instance.GetAllFriendlyCharacters().Count == 0)
        {
            // TODO (Calle): All heroes killed, do something specific to that.
            CombatManager._instance.ChangeCombatState(new CombatStateEndCombat());
        }
        else
        {
            // TODO (Calle): Continue with next turn, do we need to do anything else specific?
            CombatManager._instance.ChangeCombatState(new CombatStateTakeTurn(CombatManager._instance.GetActiveCharacter()));
        }
    }
}
