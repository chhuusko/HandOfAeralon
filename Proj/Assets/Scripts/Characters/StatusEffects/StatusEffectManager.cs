using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private List<StatusEffect> _statusEffects;
    private Character _character;
    
    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateDuration;
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

    private void UpdateDuration(Character c)
    {
        if (!_character || c != _character)
        {
            return;
        }
        
        foreach (var statusEffect in _statusEffects)
        {
            if (!statusEffect.TickDuration())
            {
                RemoveStatusEffect(statusEffect);
            }
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

    public int ModifyIncomingDamage(int damage)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyIncomingDamage(ref damage);
        }
        return damage;
    }

    public int ModifyOutgoingDamage(int damage)
    {
        foreach (var statusEffect in _statusEffects)
        {
            statusEffect.ModifyOutgoingDamage(ref damage);
        }
        return damage;
    }
}
