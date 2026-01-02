// Joel Larsson Wendt | jola6902

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AI_Executor : MonoBehaviour
{
    public UnityEvent AIEndTurn { get; private set; } = new();

    private const float TURN_START_WAIT_TIME = 1f;
    private const float TURN_END_WAIT_TIME = 2.5f;
    private const float TIMEOUT_THRESHOLD = 10f;

    public void PerformAction(AI_Context context, AI_Action action)
    {
        //Debug.Log($"AI_Executor.cs | Move to: {action.Movement.GetTileIndex()}, cast: {action.Ability.name}, at: {action.Target.GetTileIndex()}");
        StartCoroutine(Run(context, action));
    }

    public void EndTurn()
    {
        Debug.Log($"AI_Executor.cs | AI's turn ended!");
        AIEndTurn.Invoke();
    }

    private IEnumerator Run(AI_Context context, AI_Action action)
    {
        yield return new WaitForSeconds(TURN_START_WAIT_TIME);

        CharacterMovement movementComponent = null;
        if (context.Self != null && context.Self.GetCurrentHealth() > 0 && action.Movement != context.Self.GetCurrentTileComponent() && context.Self.CanMove &&
            context.Self.GetMovementPoints() > 0 && context.Self.TryGetComponent<CharacterMovement>(out movementComponent))
        {
            List<CombatGridTile> movePath =
                GridExplorer._instance.FindPathAStar(context.Self.GetCurrentTileComponent().gameObject, action.Movement.gameObject, false, context.ReachableTiles)
                .Select(obj => obj.GetComponent<CombatGridTile>())
                .Where(cgt => cgt != null)
                .ToList();

            movementComponent.ForceCustomPath(movePath);
            float timeout = 0f;

            yield return new WaitWhile(() =>
            {
                timeout += Time.deltaTime;
                if (timeout >= TIMEOUT_THRESHOLD) Debug.LogError($"AI_Executor.cs | {context.Self.name}'s movement timed out!");
                return movementComponent.IsMoving() && timeout < 10f;
            });
        }

        if (context.Self != null && context.Self.CanUseAbility && action.Ability != null && action.Target != null)
        {
            //Debug.Log($"AI_Executor.cs | {context.Self.name} tries to cast {action.Ability.name}!");
            PerformAbilityCast(context, action);
        }

        yield return new WaitForSeconds(TURN_END_WAIT_TIME);
        EndTurn();
    }

    private void PerformAbilityCast(AI_Context context, AI_Action action)
    {
        context.AbilityHandler.SetPendingAbility(action.Ability);
        context.AbilityHandler.CalculateAbilityRange();
        context.AbilityHandler.UseAbility(action.Ability, action.Target);
    }
}
