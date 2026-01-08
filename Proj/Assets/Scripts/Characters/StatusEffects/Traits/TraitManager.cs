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
    
    public bool AddStatusEffect(StatusEffect statusEffect)
    {
        StatusEffect existing = _statusEffects
            .FirstOrDefault(e => e.GetType() == statusEffect.GetType());
        
        if (existing != null)
        {
            existing.IncreaseDuration(statusEffect.Duration);
            return false;
        }
        _statusEffects.Add(statusEffect);

        if (CharacterData == null)
        {
            return true;
        }

        CharacterData.CalculateDerivedStats(CharacterData.Faction == Faction.Friendly
            ? LevelManager.GetInstance().statIncrease
            : LevelManager.GetInstance().enemyStatIncrease);
        return true;
    }

    public bool RemoveStatusEffect(StatusEffect statusEffect)
    {
        bool removed = _statusEffects.Remove(statusEffect);
        
        CharacterData.CalculateDerivedStats(CharacterData.Faction == Faction.Friendly
            ? LevelManager.GetInstance().statIncrease
            : LevelManager.GetInstance().enemyStatIncrease);
        
        return removed;
    }

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
        
        CharacterData.CalculateDerivedStats(CharacterData.Faction == Faction.Friendly
            ? LevelManager.GetInstance().statIncrease
            : LevelManager.GetInstance().enemyStatIncrease);
        
        return amount;
    }

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

    public void ModifyDerivedStats(ref float hpFactor, ref float damageFactor)
    {
        foreach (var trait in GetAllTraits())
        {
            trait.ModifyDerivedStats(ref hpFactor, ref damageFactor);
        }
    }
}
