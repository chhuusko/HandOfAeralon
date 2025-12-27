using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AI_Executor : MonoBehaviour
{
    // Singleton pattern
    private static AI_Executor Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public static AI_Executor GetInstance()
    {
        return Instance;
    }
    // End of singleton pattern

    [Tooltip("Time to wait before acting.")]
    [SerializeField] private float _turnStartWaitTime = 1f;
    [Tooltip("Time to wait after acting, before ending turn.")]
    [SerializeField] private float _turnEndWaitTime = 2.5f;
    [Tooltip("Timeout threshold for if the AI gets stuck.")]
    [SerializeField] private float _timeout = 10f;

    public void PerformAction(AI_Context context, AI_Action action)
    {
        StartCoroutine(Run(context, action));
    }

    private IEnumerator Run(AI_Context context, AI_Action action)
    {
        yield return new WaitForSeconds(_turnStartWaitTime);

        CharacterMovement movementComponent = null;
        if (context.Self != null && context.PercentHP > 0f && action.Movement != context.Self.GetCurrentTileComponent() && context.Self.CanMove &&
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
                if (timeout >= _timeout) Debug.LogError($"AI_Executor.cs | {context.Self.name}'s movement timed out!");
                return movementComponent.IsMoving() && timeout < 10f;
            });
        }

        if (context.Self != null && context.Self.CanUseAbility && action.Ability != null && action.Target != null)
        {
            //Debug.Log($"AI_Executor.cs | {_character.name} tries to cast {chosenAction.ability.name}!");
            PerformAbilityCast(context, action);
        }

        yield return new WaitForSeconds(_turnEndWaitTime);
        AI_Core.GetInstance().EndTurn();
    }

    private void PerformAbilityCast(AI_Context context, AI_Action action)
    {
        context.AbilityHandler.SetPendingAbility(action.Ability);
        context.AbilityHandler.CalculateAbilityRange();
        context.AbilityHandler.UseAbility(action.Ability, action.Target);
    }
}
