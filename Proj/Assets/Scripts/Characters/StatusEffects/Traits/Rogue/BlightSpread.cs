using System.Collections.Generic;
using UnityEngine;

public class BlightSpread : Trait
{
    private Dictionary<Character, int> _poisonDurations = new();
    
    // Applies poison to a new enemy if an enemy poisoned by this character dies.
    public override void OnDeath(Character c)
    {
        if (!_poisonDurations.TryGetValue(c, out int duration))
        {
            Debug.Log("No duration exists!");
            return;
        }

        var data = Data as FloatModifierData;
        if (!data)
        {
            Debug.Log("No data exists!");
            return;
        }
        
        _poisonDurations.Remove(c);
        
        var enemies = CombatGrid._instance.GetAllEnemyCharacters();
        enemies.Remove(c.gameObject);

        if (enemies.Count == 0)
        {
            Debug.Log("No enemies exist!");
            return;
        }
        
        var target = enemies[UnityEngine.Random.Range(0, enemies.Count)];
        target.GetComponent<Character>().GetStatusEffectManager().
            AddStatusEffect(new Poison(Mathf.RoundToInt(duration / data.Modifier)));
    }

    public override void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        // Only poisons applied by this character to enemies count.
        if (target?.GetFaction() is not Faction.Enemy || caster != Character || statusEffect is not Poison poison)
        {
            return;
        }
        
        _poisonDurations[target] = poison.Duration;
    }
}
