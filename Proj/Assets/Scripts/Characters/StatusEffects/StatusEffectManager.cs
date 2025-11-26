using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    [SerializeField] private StatusEffectDataRegistry _registry;
    
    private List<StatusEffect> _statusEffects = new();
    private Character _character;
    
    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += OnTurnStart;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateDuration;
    }

    private void Awake()
    {
        if (_registry != null)
        {
            _registry.Initialize();
        }
    }

    private void Start()
    {
        _character = GetComponent<Character>();
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateDuration;
    }
    
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (_statusEffects.Contains(statusEffect))
        {
            statusEffect.IncreaseDuration(statusEffect.Duration);
            return;
        }
        _statusEffects.Add(statusEffect);
        statusEffect.Initialize(_character, this);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        statusEffect.OnExpire();
        _statusEffects.Remove(statusEffect);
    }

    public bool ContainsStatusEffect<T>() where T : StatusEffect
    {
        return _statusEffects.Exists(e => e is T);
    }

    private void UpdateDuration(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        List<StatusEffect> statusEffectsToRemove = new();
        
        foreach (var statusEffect in _statusEffects)
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
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnApply();
        }
    }

    private void OnExpire()
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnExpire();
        }
    }

    private void OnTurnStart(Character c)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnStart();
        }
    }

    private void OnTurnEnd()
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnEnd();
        }
    }

    public float ModifyIncomingDamage(float damage, Ability ability)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyIncomingDamage(ref damage, ability);
        }
        return damage;
    }

    public float ModifyOutgoingDamage(float damage, Ability ability)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyOutgoingDamage(ref damage, ability);
        }
        return damage;
    }

    public float ModifyIncomingHeal(float heal, Ability ability)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyIncomingHeal(ref heal, ability);
        }
        return heal;
    }
    
    public float ModifyOutgoingHeal(float heal, Ability ability)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyOutgoingHeal(ref heal, ability);
        }

        return heal;
    }
}
