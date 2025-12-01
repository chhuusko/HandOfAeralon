using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatusEffects
{
    private List<StatusEffect> _statusEffects = new();
    
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (_statusEffects.Contains(statusEffect))
        {
            statusEffect.IncreaseDuration(statusEffect.Duration);
            return;
        }
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

    public IReadOnlyList<StatusEffect> GetAllStatusEffects()
    {
        return _statusEffects;
    }
    
    /// <summary>
    /// Adds one positive and one negative trait for the character.
    /// </summary>
    public void GenerateTraits()
    {
        IReadOnlyList<TraitData> positiveTraits = StatusEffectDataRegistry.Instance.GetAllTraitsOfType(true);
        IReadOnlyList<TraitData> negativeTraits = StatusEffectDataRegistry.Instance.GetAllTraitsOfType(false);

        AddStatusEffect(positiveTraits[UnityEngine.Random.Range(0, positiveTraits.Count)].CreateInstance());
        AddStatusEffect(negativeTraits[UnityEngine.Random.Range(0, negativeTraits.Count)].CreateInstance());
    }
}
