using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private Character _character;
    [SerializeField] private TraitManager _traitManager;
    
    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateDuration;
        CombatEventManager.OnAbilityDataCreated += OnAbilityUsed;
        CombatEventManager.OnEnterCombatStateEndCombat += OnCombatEnded;
        CombatEventManager.OnStatusEffectAppliedToCharacter += OnStatusEffectApplied;
        CombatEventManager.OnStatusEffectExpiredOnCharacter += OnStatusEffectRemovedFromAny;
        
        CardHandManager.onCardUse += OnCardPlayed;
        CardHandManager.onTargetCharacter += OnTargetCharacter;
    }

    private void Start()
    {
        Initialize();
        OnStartCombat();
    }
    
    private void Initialize()
    {
        _character = GetComponent<Character>();

        if (_character != null)
        {
            _character.OnTakeDamage += OnTakeDamage;
        }

        if (_traitManager == null)
        {
            _traitManager = _character.GetTraitManager();
        }

        if (_traitManager == null || !_character)
        {
            return;
        }

        // Traits need to be initialized on combat start, once character has been created.
        foreach (var trait in _traitManager.GetAllTraits())
        {
            trait.Setup(_character, this);
            trait.Initialize();
        }
    }

    public void SetTraitManager(TraitManager traitManager)
    {
        _traitManager = traitManager;
    }

    public void AddStatusEffect(StatusEffect statusEffect, Character caster = null)
    {
        // Sanctified disallows receiving debuffs.
        if (ContainsStatusEffect<Sanctified>() && statusEffect.Data.Type is StatusEffectType.Debuff)
        {
            return;
        }
        
        statusEffect.Setup(_character, this);
        
        // Check if other traits interact.
        BeforeStatusEffectApplied(caster, _character, statusEffect);
        
        // Try adding it.
        bool canAdd = CombatEventManager.InvokeOnTryAddStatusEffect(caster, _character, statusEffect);
        if (!canAdd)
        {
            statusEffect.Cleanup();
            // Blocked.
            return;
        }
        
        bool added = _traitManager.AddStatusEffect(statusEffect);
        if (added)
        {
            statusEffect.Initialize();
            CombatEventManager.InvokeOnStatusEffectAppliedToCharacter(caster, _character, statusEffect);
        }
    }

    public void RemoveStatusEffect(StatusEffect statusEffect, bool forceRemoval = false)
    {
        if (!forceRemoval && !statusEffect.Data.IsDispellable)
        {
            return;
        }
        
        bool removed = _traitManager.RemoveStatusEffect(statusEffect);

        if (!removed)
        {
            return;
        }
        
        statusEffect.Cleanup();
        statusEffect.OnExpire();
        CombatEventManager.InvokeOnStatusEffectExpiredOnCharacter(_character, statusEffect);
        OnStatusEffectRemovedFromThis(statusEffect);
    }

    public int ClearStatusEffects(StatusEffectType type)
    {
        int removed = 0;

        foreach (var effect in _traitManager.GetAllEffects().ToList())
        {
            if (effect.Data.Type == type && effect.Data.IsDispellable)
            {
                RemoveStatusEffect(effect);
                removed++;
            }
        }
        
        return removed;
    }

    public bool ContainsStatusEffect<T>() where T : StatusEffect
    {
        return _traitManager.ContainsStatusEffect<T>();
    }

    public StatusEffect GetStatusEffect<T>() where T : StatusEffect
    {
        return _traitManager.GetStatusEffect<T>();
    }

    public IReadOnlyList<StatusEffect> GetAllEffects()
    {
        return _traitManager.GetAllEffects();
    }

    public IReadOnlyList<StatusEffect> GetAllStatusEffects()
    {
        return _traitManager.GetAllStatusEffects();
    }

    public IReadOnlyList<Trait> GetAllTraits()
    {
        return _traitManager.GetAllTraits();
    }

    public IReadOnlyList<StatusEffect> GetAllOfType(StatusEffectType type)
    {
        return _traitManager.GetAllOfType(type);
    }

    private void UpdateDuration(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        List<StatusEffect> statusEffectsToRemove = new();
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects().ToList())
        {
            if (!statusEffect.TickDuration())
            {
                statusEffectsToRemove.Add(statusEffect);
            }
            CombatEventManager.InvokeOnStatusEffectDurationChanged(_character, statusEffect);
        }

        foreach (var statusEffect in statusEffectsToRemove)
        {
            RemoveStatusEffect(statusEffect);
        }
    }

    private void OnTurnStart(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.OnTurnStart();
        }
    }

    private void OnTurnEnd()
    {
        if (!_character)
        {
            return;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.OnTurnEnd();
        }
    }

    private void OnTargetCharacter(Character c)
    {
        if (!_character)
        {
            return;
        } 

        if (c != _character)
        {
            return;
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.OnTargetedByCard();
        }
    }

    public void OnBurnApplied(Character c)
    {
        if (c != _character)
        {
            return;
        }

        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.OnBurnApplied();
        }
    }

    private void OnCombatEnded(bool playerWon)
    {
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnCombatEnded();
        }

        // Clear all status effects.
        ClearStatusEffects(StatusEffectType.Buff);
        ClearStatusEffects(StatusEffectType.Debuff);
    }

    public float ModifyIncomingDamage(float damage, Ability ability)
    {
        if (!_character)
        {
            return damage;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyIncomingDamage(ref damage, ability);
        }

        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            if (statusEffect.ShouldExpire)
            {
                RemoveStatusEffect(statusEffect, true);
            }
        }
        
        return damage;
    }

    public float ModifyOutgoingDamage(float damage, Ability ability)
    {
        if (!_character)
        {
            return damage;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyOutgoingDamage(ref damage, ability);
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            if (statusEffect.ShouldExpire)
            {
                RemoveStatusEffect(statusEffect);
            }
        }
        
        return damage;
    }

    public float ModifyIncomingHeal(float heal, Ability ability)
    {
        if (!_character)
        {
            return heal;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyIncomingHeal(ref heal, ability);
        }
        return heal;
    }
    
    public float ModifyOutgoingHeal(float heal, Ability ability)
    {
        if (!_character)
        {
            return heal;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyOutgoingHeal(ref heal, ability);
        }

        return heal;
    }
    
    public int ApplyBurnDamageModifiers(int baseDamage)
    {
        int damage = baseDamage;

        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyBurnDamage(ref damage);
        }
        
        return damage;
    }
    
    // Status effects.
    /// <summary>
    /// Tries applying the burn to the target, with chance influenced by all this character's modifiers.
    /// </summary>
    /// <returns>The applied burn, or null if no burn was applied.</returns>
    public Burn TryApplyBurn(Character target, float baseChance, int duration)
    {
        float finalChance = baseChance;
        
        ApplyBurnApplicationChanceModifiers(ref finalChance);

        if (UnityEngine.Random.value < finalChance)
        {
            Burn burn = new Burn(_character, duration);
            target.GetStatusEffectManager().AddStatusEffect(burn);
            OnBurnApplied(_character);
            return burn;
        }
        return null;
    }

    /// <summary>
    /// Tries applying stun to the target character, based on the base chance.
    /// </summary>
    /// <param name="target">The target character.</param>
    /// <param name="baseChance">The base chance of stun to succeed.</param>
    /// <param name="duration">The amount of turns for the target to be stunned.</param>
    /// <returns>The applied stun, or null if no stun was applied.</returns>
    public Stunned TryApplyStun(Character target, float baseChance, int duration)
    {
        float finalChance = baseChance;
        
        ApplyStunApplicationChanceModifiers(ref finalChance);

        if (UnityEngine.Random.value < finalChance)
        {
            Stunned stun = new Stunned(duration);
            target.GetStatusEffectManager().AddStatusEffect(stun);
            return stun;
        }
        return null;
    }
    
    private float ApplyBurnApplicationChanceModifiers(ref float baseChance)
    {
        float chance = baseChance;

        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyBurnApplicationChance(ref chance);
        }
        
        return chance;
    }

    private float ApplyStunApplicationChanceModifiers(ref float baseChance)
    {
        float chance = baseChance;

        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            statusEffect.ModifyStunApplicationChance(ref chance);
        }

        return chance;
    }
    
    // Traits.
    private void OnStartCombat()
    {
        if (!_character)
        {
            return;
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects().ToList())
        {
            if (statusEffect is Trait trait)
            {
                trait.OnCombatStarted();
            }
        } 
    }
    
    private void OnTakeDamage(int damage, GameObject c)
    {
        if (!_character)
        {
            return;
        }
        
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnTakeDamage();
        }
    }

    private void OnDeath(Character c)
    {
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnDeath(c);
        }
    }

    private void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (!_character)
        {
            return;
        }
        
        if (abilityData.Caster != _character)
        {
            return;
        }
        
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnAbilityUsed(abilityData);
        }
    }

    private void BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != _character)
        {
            return;
        }
        
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.BeforeStatusEffectApplied(caster, target, statusEffect);
        }
    }
    
    private void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnStatusEffectApplied(caster, target, statusEffect);
        }
    }

    private void OnStatusEffectRemovedFromThis(StatusEffect statusEffect)
    {
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnStatusEffectRemovedFromThis(statusEffect);
        }
    }
    
    private void OnStatusEffectRemovedFromAny(Character character, StatusEffect statusEffect)
    {
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnStatusEffectRemovedFromAny(character, statusEffect);
        }
    }

    private void OnCardPlayed(Card card)
    {
        foreach (var statusEffect in _traitManager.GetAllStatusEffects().ToList())
        {
            statusEffect.OnCardPlayed(card);
        }
        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.OnCardPlayed(card);
        }
    }
    
    public int ApplyAoEModifiers(ref int baseAoE)
    {
        int AoE = baseAoE;

        foreach (var trait in _traitManager.GetAllTraits().ToList())
        {
            trait.ModifyAoE(ref AoE);
        }
        
        return AoE;
    }
    
    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateDuration;
        CombatEventManager.OnAbilityDataCreated -= OnAbilityUsed;
        CombatEventManager.OnEnterCombatStateEndCombat -= OnCombatEnded;
        CombatEventManager.OnStatusEffectAppliedToCharacter -= OnStatusEffectApplied;
        CombatEventManager.OnStatusEffectExpiredOnCharacter -= OnStatusEffectRemovedFromAny;
        
        CardHandManager.onCardUse -= OnCardPlayed;
        CardHandManager.onTargetCharacter -= OnTargetCharacter;

        _character.OnTakeDamage -= OnTakeDamage;
    }
}
