// Joel Larsson Wendt | jola6902

using System.Collections.Generic;
using UnityEngine;

public class AI_Core
{
    private AI_Searcher _searcher;
    private AI_Evaluator _evaluator;
    private AI_Executor _executor;
    private Faction _faction;

    private Character _currentCharacter = null;

    public static AI_Core BuildAICore(Faction faction)
    {
        DebugLog.JLWLog("AI_Core built.");

        GameObject monobehaviour = new GameObject("AI_Executor");
        AI_Executor executor = monobehaviour.AddComponent<AI_Executor>();

        AI_Core result = new AI_Core(
            new AI_Searcher(),
            new AI_Evaluator(),
            executor,
            faction
            );

        return result;
    }

    public AI_Core(AI_Searcher searcher, AI_Evaluator evaluator, AI_Executor executor, Faction faction)
    {
        _searcher = searcher;
        _evaluator = evaluator;
        _executor = executor;
        _faction = faction;

        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
    }

    public UnityEngine.Events.UnityEvent GetAIEndTurnEvent()
    {
        return _executor.AIEndTurn;
    }

    public void OnTurnStart(Character character)
    {
        if (TurnStartedProperly())
        {
            DebugLog.JLWLog("Turn started properly.");
            Run();
        }
    }

    private bool TurnStartedProperly()
    {
        _currentCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();

        if (_currentCharacter == null || _currentCharacter.GetFaction() != _faction)
        {
            return false;
        }

        if (_currentCharacter.IsStunned)
        {
            Debug.Log($"AI_Core.cs | {_currentCharacter.name} was Stunned and will pass their turn.");
            _executor.EndTurn();
            return false;
        }

        return true;
    }

    private void Run()
    {
        AI_Context context = new AI_Context(_currentCharacter);
        List<AI_Action> actions = _searcher.GetPossibleActions(context);
        AI_Action best = _evaluator.Evaluate(context, actions);
        _executor.PerformAction(context, best);
    }
}
