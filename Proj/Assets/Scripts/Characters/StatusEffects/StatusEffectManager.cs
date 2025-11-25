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
        _statusEffects.Add(statusEffect);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
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

    public void OnApply()
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnApply();
        }
    }

    public void OnExpire()
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnExpire();
        }
    }

    public void OnTurnStart()
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnStart();
        }
    }

    public void OnTurnEnd()
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.OnTurnEnd();
        }
    }

    public float ModifyIncomingDamage(float damage)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyIncomingDamage(ref damage);
        }
        return damage;
    }

    public float ModifyOutgoingDamage(float damage)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyOutgoingDamage(ref damage);
        }
        return damage;
    }
}
