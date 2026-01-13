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
    public static event Action<Character> OnEnterCombatStateEndTurn;
    public static event Action<bool> OnEnterCombatStateEndCombat;

    // Exit Combat States
    public static event Action OnExitCombatStateLoadNextLevel;
    public static event Action OnExitCombatStateIntroCinematic;
    public static event Action OnExitCombatStatePlaceCharacter;
    public static event Action OnExitCombatStateTakeTurn;
    public static event Action OnExitCombatStateEndTurn;
    public static event Action<bool> OnExitCombatStateEndCombat;

    public static event Action<bool> OnIsHoveringUI;

    public static event Action<IReadOnlyList<Character>, int> OnTurnOrderChanged;

    public static event Action<int> OnRoundFinished;

    public static event Action<Character> OnCharacterSpawned;
    public static event Action<Character> OnCharacterDeath;
    public static event Action OnCharacterInitiativeChanged;

    public static event Action OnCharacterPlaced;

    public static event Action<AbilityExecutionData> OnAbilityDataCreated;

    public static event Action BeforeAbilityCast;
    public static event Action<Character, Ability> AfterAbilityCast;
    
    public static event Action<Character, bool> OnCharacterMove;

    // Status effects.
    public static event Action<Character, Character, StatusEffect> OnStatusEffectAppliedToCharacter;
    public static event Action<Character, StatusEffect> OnStatusEffectExpiredOnCharacter;
    public static event Action<Character, StatusEffect> OnStatusEffectDurationChanged;
    public static event Action<Character, StatusEffect, int> OnStatusEffectDamageDealt;

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

    public static void InvokeEnterCombatStateEndTurn(Character activeCharacter)
        => OnEnterCombatStateEndTurn?.Invoke(activeCharacter);
    public static void InvokeEnterCombatStateEndCombat(bool playerWon)
        => OnEnterCombatStateEndCombat?.Invoke(playerWon);

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
    public static void InvokeExitCombatStateEndCombat(bool playerWon)
        => OnExitCombatStateEndCombat?.Invoke(playerWon);

    public static void InvokeOnIsHoveringUI(bool mouseIsHoveringUI)
        => OnIsHoveringUI?.Invoke(mouseIsHoveringUI);

    public static void InvokeOnCharacterDeath(Character character)
        => OnCharacterDeath?.Invoke(character);
    
    public static void InvokeOnCharacterSpawned(Character character) 
        => OnCharacterSpawned?.Invoke(character);

    public static void InvokeOnCharacterInitiativeChanged()
       => OnCharacterInitiativeChanged?.Invoke();
    
    public static void InvokeOnTurnOrderChanged(IReadOnlyList<Character> characterTurnOrder, int currentRound)
        => OnTurnOrderChanged?.Invoke(characterTurnOrder, currentRound);

    public static void InvokeOnRoundFinished(int currentRound)
        => OnRoundFinished?.Invoke(currentRound);

    public static void InvokeOnCharacterPlaced() 
        => OnCharacterPlaced?.Invoke();
    
    public static void InvokeOnAbilityDataCreated(AbilityExecutionData result)
       => OnAbilityDataCreated?.Invoke(result);

    public static void InvokeBeforeAbilityCast()
       => BeforeAbilityCast?.Invoke();
    
    public static void InvokeAfterAbilityCast(Character character, Ability ability)
        => AfterAbilityCast?.Invoke(character, ability);
    
    public static void InvokeOnCharacterMove(Character character, bool isMoving)
        => OnCharacterMove?.Invoke(character, isMoving);

    public static void InvokeOnStatusEffectAppliedToCharacter(Character caster, Character target, StatusEffect statusEffect)
        => OnStatusEffectAppliedToCharacter?.Invoke(caster, target, statusEffect);

    public static void InvokeOnStatusEffectExpiredOnCharacter(Character character, StatusEffect statusEffect)
    => OnStatusEffectExpiredOnCharacter?.Invoke(character, statusEffect);

    public static void InvokeOnStatusEffectDurationChanged(Character character, StatusEffect statusEffect)
    => OnStatusEffectDurationChanged?.Invoke(character, statusEffect);

    public static void InvokeOnStatusEffectDamageDealt(Character character, StatusEffect statusEffect, int damage) 
    => OnStatusEffectDamageDealt?.Invoke(character, statusEffect, damage);
}
