using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TraitManager
{
    [NonSerialized] public CharacterData CharacterData;
    
    [SerializeReference] private List<StatusEffect> _statusEffects = new();
    public IReadOnlyList<StatusEffect> StatusEffects => _statusEffects;
    
    /// <summary>
    /// Adds a status effect to the character, or increases duration if already existing.
    /// </summary>
    /// <param name="statusEffect">The status effect to add.</param>
    /// <returns>Whether the status effect has been added.</returns>
    public bool AddStatusEffect(StatusEffect statusEffect)
    {
        StatusEffect existing = _statusEffects
            .FirstOrDefault(e => e.GetType() == statusEffect.GetType());
        
        if (existing != null)
        {
            int longestDuration = statusEffect.Duration > existing.Duration ? statusEffect.Duration : existing.Duration;
            existing.SetDuration(longestDuration);
            return false;
        }
        _statusEffects.Add(statusEffect);

        if (CharacterData == null)
        {
            return true;
        }

        CharacterData.RecalculateStatusModifiers();
        return true;
    }

    /// <summary>
    /// Removes the given status effect from the character.
    /// </summary>
    /// <param name="statusEffect">The status effect to remove.</param>
    /// <returns>Whether the status effect has been removed.</returns>
    public bool RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (statusEffect is Trait)
        {
            return false;
        }
        
        bool removed = _statusEffects.Remove(statusEffect);
        
        CharacterData.RecalculateStatusModifiers();
        
        return removed;
    }

    /// <summary>
    /// Removes all status effects of the given type.
    /// </summary>
    /// <param name="type">The type of status effect to remove.</param>
    /// <returns>The amount of status effects removed.</returns>
    public int ClearStatusEffects(StatusEffectType type)
    {
        int amount = 0;
        List<StatusEffect> statusEffectsToRemove = new();

        foreach (var statusEffect in _statusEffects)
        {
            if (statusEffect.Data.Type == type && statusEffect.Data.IsDispellable)
            {
                statusEffectsToRemove.Add(statusEffect);
                amount++;
            }
        }

        foreach (var statusEffect in statusEffectsToRemove)
        {
            _statusEffects.Remove(statusEffect);
        }
        
        CharacterData.RecalculateStatusModifiers();
        
        return amount;
    }

    /// <summary>
    /// Finds all traits the character currently has.
    /// </summary>
    /// <returns>A list of all traits on the character.</returns>
    public IReadOnlyList<Trait> GetAllTraits()
    {
        List<Trait> all = new();
        foreach (var statusEffect in _statusEffects)
        {
            if (statusEffect is Trait trait)
            {
                all.Add(trait);
            }
        }

        return all;
    }

    /// <summary>
    /// Finds all status effects of the given type that the character currently has.
    /// </summary>
    /// <param name="type">The type of status effect to get.</param>
    /// <returns>All status effects of the given type.</returns>
    public IReadOnlyList<StatusEffect> GetAllOfType(StatusEffectType type)
    {
        return (IReadOnlyList<StatusEffect>)_statusEffects.Where(e => e.Data.Type == type);
    }
    
    /// <summary>
    /// Adds one positive and one negative trait for the character.
    /// </summary>
    public void GenerateTraits(CharacterData character)
    {
        var positiveTraits = 
            UnityEngine.Random.value <= GlobalGameManager.GetInstance().ClassTraitChancePercent / 100f ? 
                StatusEffectDataRegistry.Instance.GetAllClassTraits(character) :
                StatusEffectDataRegistry.Instance.GetAllGlobalTraitsOfType(true);
        
        IReadOnlyList<StatusEffectData> negativeTraits = StatusEffectDataRegistry.Instance.GetAllGlobalTraitsOfType(false);

        if (positiveTraits.Count > 0)
        {
            AddStatusEffect(positiveTraits[UnityEngine.Random.Range(0, positiveTraits.Count)].CreateInstance());
        }
        
        if (negativeTraits.Count > 0)
        {
            AddStatusEffect(negativeTraits[UnityEngine.Random.Range(0, negativeTraits.Count)].CreateInstance());
        }
    }

    /// <summary>
    /// Applies hp and damage scaling on the character for each trait it has.
    /// </summary>
    /// <param name="hpFactor">The HP scaling factor to modify.</param>
    /// <param name="damageFactor">The damage scaling factor to modify.</param>
    public void ModifyDerivedStats(ref float hpFactor, ref float damageFactor)
    {
        foreach (var trait in GetAllTraits())
        {
            trait.ModifyDerivedStats(ref hpFactor, ref damageFactor);
        }
    }
}
