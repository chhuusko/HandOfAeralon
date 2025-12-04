using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        CardHandManager.onTargetCharacter += OnCardPlayed;
    }

    private void Start()
    {
        Initialize();
        OnStartCombat();
    }

    private void Initialize()
    {
        _character = GetComponent<Character>();

        if (_traitManager == null)
        {
            _traitManager = _character.GetTraitManager();
        }
        
        // AddStatusEffect(new Stealth(3));
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateDuration;
        CombatEventManager.OnAbilityDataCreated -= OnAbilityUsed;
        CombatEventManager.OnEnterCombatStateEndCombat -= OnCombatEnded;
        
        CardHandManager.onTargetCharacter -= OnCardPlayed;
    }

    public void SetTraitManager(TraitManager traitManager)
    {
        _traitManager = traitManager;
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        // Sanctified disallows receiving debuffs.
        if (ContainsStatusEffect<Sanctified>() && statusEffect.Data.Type is StatusEffectType.Debuff)
        {
            return;
        }
        
        _traitManager.AddStatusEffect(statusEffect);
        statusEffect.Initialize(_character, this);
        CombatEventManager.InvokeOnStatusEffectAppliedToCharacter(_character, statusEffect);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffect.OnExpire();
        _traitManager.RemoveStatusEffect(statusEffect);
        CombatEventManager.InvokeOnStatusEffectExpiredOnCharacter(_character, statusEffect);
    }

    public int ClearStatusEffects(StatusEffectType type)
    {
        return _traitManager.ClearStatusEffects(type);
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

    private void UpdateDuration(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        List<StatusEffect> statusEffectsToRemove = new();
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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

    private void OnApply()
    {
        if (!_character)
        {
            return;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.OnApply();
        }
    }

    private void OnExpire()
    {
        if (!_character)
        {
            return;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.OnExpire();
        }
    }

    private void OnTurnStart(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.OnTurnEnd();
        }
    }

    private void OnCardPlayed(Character c)
    {
        if (!_character)
        {
            return;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.OnCardPlayed();
        }

        if (c != _character)
        {
            return;
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.OnTargetedByCard();
        }
    }

    private void OnBurnApplied(Character c)
    {
        if (c != _character)
        {
            return;
        }

        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.OnBurnApplied();
        }
    }

    public void OnCombatEnded()
    {
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            if (statusEffect is Trait trait)
            {
                trait.OnStartCombat();
            }
        } 
    }

    public float ModifyIncomingDamage(float damage, Ability ability)
    {
        if (!_character)
        {
            return damage;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.ModifyIncomingDamage(ref damage, ability);
        }
        return damage;
    }

    public float ModifyOutgoingDamage(float damage, Ability ability)
    {
        if (!_character)
        {
            return damage;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.ModifyOutgoingDamage(ref damage, ability);
        }
        return damage;
    }

    public float ModifyIncomingHeal(float heal, Ability ability)
    {
        if (!_character)
        {
            return heal;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            statusEffect.ModifyOutgoingHeal(ref heal, ability);
        }

        return heal;
    }
    
    // Traits.
    private void OnStartCombat()
    {
        if (!_character)
        {
            return;
        }
        
        foreach (var statusEffect in _traitManager.GetAllEffects())
        {
            if (statusEffect is Trait trait)
            {
                trait.OnStartCombat();
            }
        } 
    }
    
    public void OnTakeDamage()
    {
        if (!_character)
        {
            return;
        }
        
        foreach (var trait in _traitManager.GetAllTraits())
        {
            trait.OnTakeDamage();
        }
    }

    private void OnAbilityUsed(AbilityExecutionData data)
    {
        if (!_character)
        {
            return;
        }
        
        if (data.Caster != _character)
        {
            return;
        }
        
        foreach (var trait in _traitManager.GetAllTraits())
        {
            trait.OnAbilityUsed(data.Ability);
        }
    }
}
