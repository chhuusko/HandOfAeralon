using System.Collections.Generic;
using UnityEngine;

public class BlightSpread : Trait
{
    private List<Character> _targets;
    
    // Applies poison to a new enemy if an enemy poisoned by this character dies.
    public override void OnDeath(Character c)
    {
        if (c.GetFaction() is not Faction.Enemy || !_targets.Contains(c) || 
            !c.GetStatusEffectManager().ContainsStatusEffect<Poison>())
        {
            return;
        }

        var data = Data as FloatModifierData;

        if (!data)
        {
            return;
        }
        
        int duration = c.GetStatusEffectManager().GetStatusEffect<Poison>().Duration;
        
        _targets.Remove(c);
        
        var enemies = CombatGrid._instance.GetAllEnemyCharacters();
        enemies[UnityEngine.Random.Range(0, enemies.Count)]?.GetComponent<Character>()?.GetStatusEffectManager()?.
            AddStatusEffect(new Poison(Mathf.RoundToInt(duration / data.Modifier)));
    }

    public override void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        // Only poisons applied by this character to enemies count.
        if (target?.GetFaction() is not Faction.Enemy || caster != Character || statusEffect is not Poison)
        {
            return;
        }
        
        _targets.Add(target);
    }

    public override void OnStatusEffectRemovedFromAny(Character character, StatusEffect statusEffect)
    {
        _targets.Remove(character);
    }
}
