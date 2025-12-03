using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class CombatEventManager
{
    public static event Action<CombatState> OnCombatStateChange;
    public static event Action<CombatTurn> OnCombatTurnChange;

    // Enter Combat States
    public static event Action OnEnterCombatStateLoadNextLevel;
    public static event Action OnEnterCombatStateIntroCinematic;
    public static event Action OnEnterCombatStatePlaceCharacter;
    public static event Action<Character> OnEnterCombatStateTakeTurn;
    public static event Action OnEnterCombatStateEndTurn;
    public static event Action OnEnterCombatStateEndCombat;

    // Exit Combat States
    public static event Action OnExitCombatStateLoadNextLevel;
    public static event Action OnExitCombatStateIntroCinematic;
    public static event Action OnExitCombatStatePlaceCharacter;
    public static event Action OnExitCombatStateTakeTurn;
    public static event Action OnExitCombatStateEndTurn;
    public static event Action OnExitCombatStateEndCombat;

    public static event Action<IReadOnlyList<Character>> OnTurnOrderChanged;

    public static event Action<Character> OnCharacterDeath;

    public static event Action<AbilityExecutionData> OnAbilityDataCreated;

    public static event Action OnAbilityCast;
    
    public static event Action<bool> OnCharacterMove;

    public static event Action<Character, StatusEffect> OnStatusEffectAppliedToCharacter;
    public static event Action<Character, StatusEffect> OnStatusEffectExpiredOnCharacter;
    public static event Action<Character, StatusEffect> OnStatusEffectDurationChanged;

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

    public static void InvokeEnterCombatStateTakeTurn(Character activeCharacter)
        => OnEnterCombatStateTakeTurn?.Invoke(activeCharacter);

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

    public static void InvokeOnCharacterDeath(Character character)
        => OnCharacterDeath?.Invoke(character);

    public static void InvokeOnTurnOrderChanged(IReadOnlyList<Character> characterTurnOrder)
        => OnTurnOrderChanged?.Invoke(characterTurnOrder);

    public static void InvokeOnAbilityDataCreated(AbilityExecutionData result)
       => OnAbilityDataCreated?.Invoke(result);

    public static void InvokeOnAbilityCast()
       => OnAbilityCast?.Invoke();
    
    public static void InvokeOnCharacterMove(bool isMoving)
        => OnCharacterMove?.Invoke(isMoving);

    public static void InvokeOnStatusEffectAppliedToCharacter(Character character, StatusEffect statusEffect)
        => OnStatusEffectAppliedToCharacter?.Invoke(character, statusEffect);

    public static void InvokeOnStatusEffectExpiredOnCharacter(Character character, StatusEffect statusEffect)
    => OnStatusEffectExpiredOnCharacter?.Invoke(character, statusEffect);

    public static void InvokeOnStatusEffectDurationChanged(Character character, StatusEffect statusEffect)
    => OnStatusEffectDurationChanged?.Invoke(character, statusEffect);

}
