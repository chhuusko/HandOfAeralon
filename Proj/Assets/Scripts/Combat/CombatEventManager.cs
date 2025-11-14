using System;
using UnityEngine;

public static class CombatEventManager
{
    public static event Action<CombatState> OnCombatStateChange;
    public static event Action<CombatTurn> OnCombatTurnChange;

    public static void CombatStateChanged(CombatState newState)
        => OnCombatStateChange?.Invoke(newState);

    public static void CombatTurnChanged(CombatTurn turn)
        => OnCombatTurnChange?.Invoke(turn);
}
