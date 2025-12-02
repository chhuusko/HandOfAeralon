using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    [SerializeField] private float _classTraitChance;
    
    private Character _character;
    private TraitManager _traitManager;
    
    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateDuration;
        CombatEventManager.OnAbilityDataCreated += OnAbilityUsed;
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
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateDuration;
        CombatEventManager.OnAbilityDataCreated -= OnAbilityUsed;
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
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffect.OnExpire();
        _traitManager.RemoveStatusEffect(statusEffect);
    }

    public bool ContainsStatusEffect<T>() where T : StatusEffect
    {
        return _traitManager.ContainsStatusEffect<T>();
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
        {
            statusEffect.OnTurnEnd();
        }
    }

    private void OnCardUsed()
    {
        if (!_character)
        {
            return;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
        {
            statusEffect.OnCardPlayed();
        }
    }

    public float ModifyIncomingDamage(float damage, Ability ability)
    {
        if (!_character)
        {
            return damage;
        } 
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
        {
            statusEffect.ModifyOutgoingHeal(ref heal, ability);
        }

        return heal;
    }
    
    // Traits.
    private void OnStartCombat()
    {
        foreach (var statusEffect in _traitManager.GetAllStatusEffects())
        {
            if (statusEffect is Trait trait)
            {
                trait.OnStartCombat();
            }
        } 
    }
    
    public void OnTakeDamage()
    {
        foreach (var trait in _traitManager.GetAllTraits())
        {
            trait.OnTakeDamage();
        }
    }

    private void OnAbilityUsed(AbilityExecutionData data)
    {
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
