using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private Character _character;
    private CharacterStatusEffects _statusEffects;
    
    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateDuration;
    }

    private void Awake()
    {
        _statusEffects = new CharacterStatusEffects();
    }

    private void Start()
    {
        _character = GetComponent<Character>();
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateDuration;
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        _statusEffects.AddStatusEffect(statusEffect);
        statusEffect.OnApply();
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffect.OnExpire();
        _statusEffects.RemoveStatusEffect(statusEffect);
    }

    public bool ContainsStatusEffect<T>() where T : StatusEffect
    {
        return _statusEffects.ContainsStatusEffect<T>();
    }

    public IReadOnlyList<StatusEffect> GetAllStatusEffects()
    {
        return _statusEffects.GetAllStatusEffects();
    }

    private void UpdateDuration(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        List<StatusEffect> statusEffectsToRemove = new();
        
        // TODO: Don't tick permanent status effects.
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
        {
            statusEffect.OnTurnEnd();
        }
    }

    public float ModifyIncomingDamage(float damage, Ability ability)
    {
        if (!_character)
        {
            return damage;
        } 
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
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
        
        foreach (var statusEffect in _statusEffects.GetAllStatusEffects())
        {
            statusEffect.ModifyOutgoingHeal(ref heal, ability);
        }

        return heal;
    }
}
