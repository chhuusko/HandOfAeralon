using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TraitManager
{
    [NonSerialized] public CharacterData CharacterData;
    
    [SerializeReference] private List<StatusEffect> _statusEffects = new();
    
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        StatusEffect existing = _statusEffects
            .FirstOrDefault(e => e.GetType() == statusEffect.GetType());
        
        if (existing != null)
        {
            existing.IncreaseDuration(statusEffect.Duration);
            return;
        }
        _statusEffects.Add(statusEffect);

        if (CharacterData == null)
        {
            return;
        }

        CharacterData.CalculateDerivedStats(CharacterData.Faction == Faction.Friendly
            ? LevelManager.GetInstance().statIncrease
            : LevelManager.GetInstance().enemyStatIncrease);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        _statusEffects.Remove(statusEffect);
        
        CharacterData.CalculateDerivedStats(CharacterData.Faction == Faction.Friendly
            ? LevelManager.GetInstance().statIncrease
            : LevelManager.GetInstance().enemyStatIncrease);
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

    public bool ContainsStatusEffect<T>() where T : StatusEffect
    {
        return _statusEffects.Exists(e => e is T);
    }

    public StatusEffect GetStatusEffect<T>() where T : StatusEffect
    {
        return _statusEffects.Find(e => e.GetType() == typeof(T));
    }

    public IReadOnlyList<StatusEffect> GetAllEffects()
    {
        return _statusEffects;
    }

    public IReadOnlyList<StatusEffect> GetAllStatusEffects()
    {
        return _statusEffects.Where(e => e is not Trait).ToList();
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
    
    /// <summary>
    /// Adds one positive and one negative trait for the character.
    /// </summary>
    public void GenerateTraits(CharacterData character)
    {
        IReadOnlyList<StatusEffectData> positiveTraits;
        
        if (UnityEngine.Random.value >= GlobalGameManager.GetInstance().ClassTraitChance)
        {
            positiveTraits = StatusEffectDataRegistry.Instance.GetAllGlobalTraitsOfType(true);
        }
        else
        {
            positiveTraits = StatusEffectDataRegistry.Instance.GetAllClassTraits(character);
        }
        
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
