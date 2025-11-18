using System;
using UnityEngine;

public static class CombatEventManager
{
    public static event Action<CombatState> OnCombatStateChange;
    public static event Action<CombatState> OnCombatEnterState;
    public static event Action<CombatTurn> OnCombatTurnChange;
    
    public static event Action<CombatState> OnCombatStateEnter;
    public static event Action<CombatState> OnCombatStateExit;
    
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


    public static void CombatStateChanged(CombatState newState)
        => OnCombatStateChange?.Invoke(newState);

    public static void CombatTurnChanged(CombatTurn turn)
        => OnCombatTurnChange?.Invoke(turn);

    public static void InvokeEnterCombatStateLoadNextLevel()
        => OnEnterCombatStateLoadNextLevel?.Invoke();

    public static void InvokeEnterCombatStateIntroCinematic()
        => OnEnterCombatStateIntroCinematic?.Invoke();

    public static void InvokeEnterCombatStatePlaceCharacter()
        => OnEnterCombatStatePlaceCharacter.Invoke();

}
