// Joel Larsson Wendt || jola6902

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AI_Core : MonoBehaviour
{
    // Singleton pattern
    private static AI_Core Instance;

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

    public static AI_Core GetInstance()
    {
        return Instance;
    }
    // End of singleton pattern

    public UnityEvent AIEndTurn;

    [SerializeField] private Faction _controlledFaction = Faction.Enemy;

    private Character _currentCharacter = null;

    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= OnTurnStart;
    }

    private void OnTurnStart(Character character)
    {
        if (TurnStartedProperly())
        {
            Run();
        }
    }

    private bool TurnStartedProperly()
    {
        _currentCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();

        if (_currentCharacter == null || _currentCharacter.GetFaction() != _controlledFaction)
        {
            return false;
        }

        if (_currentCharacter.IsStunned)
        {
            //Debug.Log($"AI_Core.cs | {_currentCharacter.name} was Stunned and will pass their turn.");
            EndTurn();
            return false;
        }

        return true;
    }

    private void Run()
    {
        AI_Context context = new AI_Context(_currentCharacter);
        List<AI_Action> actions = AI_Searcher.GetInstance().GetPossibleActions(context);
        AI_Action best = AI_Evaluator.GetInstance().Evaluate(context, actions);
        AI_Executor.GetInstance().PerformAction(context, best);
    }

    public void EndTurn()
    {
        //Debug.Log($"AI_Core.cs | {_currentCharacter.name}'s turn ended!");
        _currentCharacter = null;
        AIEndTurn.Invoke();
    }
}
