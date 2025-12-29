using System.Collections.Generic;
using UnityEngine;

public class ChainReaction : Trait
{
    private HashSet<Character> _appliedThisTurn = new();

    public override void OnTurnStart()
    {
        _appliedThisTurn.Clear();
    }

    public override void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (caster != Character || !target || target.GetFaction() == caster.GetFaction() 
            || _appliedThisTurn.Contains(target) || statusEffect is not Burn)
        {
            return;
        }
        
        var data = Data as ChainReactionData;
        if (!data)
        {
            return;
        }
        
        var tilesInRange = GridExplorer._instance.GetTilesInRange(
            target.GetCurrentTileComponent().gameObject, data.Range, false);
        List<Character> enemiesInRange = new();

        foreach (var tile in tilesInRange)
        {
            var occupant = tile.GetComponent<CombatGridTile>().GetOccupant();
            if (!occupant)
            {
                continue;
            }

            var character = occupant.GetComponent<Character>();
            if (character.GetFaction() != caster.GetFaction())
            {
                enemiesInRange.Add(character);
            }
        }

        if (enemiesInRange.Count == 0)
        {
            return;
        }
        
        float applicationChance = data.ApplicationChancePercent / 100f;
        if (UnityEngine.Random.value <= applicationChance)
        {
            var character = enemiesInRange[UnityEngine.Random.Range(0, enemiesInRange.Count)];
            _appliedThisTurn.Add(character);
            character.GetStatusEffectManager().
                AddStatusEffect(new Burn(Character, data.Duration));
        }
    }
}
