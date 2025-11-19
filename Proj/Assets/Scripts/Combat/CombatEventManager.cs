using System;
using UnityEngine;

public static class CombatEventManager
{
    public static event Action<CombatState> OnCombatStateChange;
    public static event Action<CombatTurn> OnCombatTurnChange;
    
    public static event Action OnEnterCombatStateLoadNextLevel;
    public static event Action OnEnterCombatStateIntroCinematic;
    public static event Action OnEnterCombatStatePlaceCharacter;
    public static event Action OnEnterCombatStateTakeTurn;
    public static event Action OnEnterCombatStateEndTurn;
    public static event Action OnEnterCombatStateEndCombat;

    public static event Action OnExitCombatStateLoadNextLevel;
    public static event Action OnExitCombatStateIntroCinematic;
    public static event Action OnExitCombatStatePlaceCharacter;
    public static event Action OnExitCombatStateTakeTurn;
    public static event Action OnExitCombatStateEndTurn;
    public static event Action OnExitCombatStateEndCombat;


    public static void InvokeCombatStateChanged(CombatState newState)
        => OnCombatStateChange?.Invoke(newState);

    public static void InvokeCombatTurnChanged(CombatTurn turn)
        => OnCombatTurnChange?.Invoke(turn);

    public static void InvokeEnterCombatStateLoadNextLevel()
        => OnEnterCombatStateLoadNextLevel?.Invoke();

    public static void InvokeEnterCombatStateIntroCinematic()
        => OnEnterCombatStateIntroCinematic?.Invoke();

    public static void InvokeEnterCombatStatePlaceCharacter()
        => OnEnterCombatStatePlaceCharacter?.Invoke();

    public static void InvokeEnterCombatStateTakeTurn()
        => OnEnterCombatStateTakeTurn?.Invoke();

    public static void InvokeEnterCombatStateEndTurn()
        => OnEnterCombatStateEndTurn?.Invoke();
    public static void InvokeEnterCombatStateEndCombat()
        => OnEnterCombatStateEndCombat?.Invoke();

    public static void InvokeExitCombatStateLoadNextLevel()
    => OnExitCombatStateLoadNextLevel?.Invoke();

    public static void InvokeExitCombatStateIntroCinematic()
        => OnExitCombatStateIntroCinematic?.Invoke();

    public static void InvokeExitCombatStatePlaceCharacter()
        => OnExitCombatStatePlaceCharacter?.Invoke();

    public static void InvokeExitCombatStateTakeTurn()
        => OnExitCombatStateTakeTurn?.Invoke();

    public static void InvokeExitCombatStateEndTurn()
        => OnExitCombatStateEndTurn?.Invoke();
    public static void InvokeExitCombatStateEndCombat()
        => OnExitCombatStateEndCombat?.Invoke();


}
