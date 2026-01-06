using System.Collections.Generic;
using UnityEngine;

public class ChainReaction : Trait
{
    private HashSet<Character> _appliedThisTurn = new();

    public override void ResetCombatState()
    {
        _appliedThisTurn.Clear();
    }

    public override void OnTurnStart()
    {
        _appliedThisTurn.Clear();
    }

    public override void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (caster != Character || !target || target.GetFaction() == caster.GetFaction() || statusEffect is not Burn)
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
        List<Character> candidates = new();

        foreach (var tile in tilesInRange)
        {
            var occupant = tile.GetComponent<CombatGridTile>().GetOccupant();
            if (!occupant)
            {
                continue;
            }

            var candidate = occupant.GetComponent<Character>();
            if (candidate.GetFaction() != caster.GetFaction() && !_appliedThisTurn.Contains(candidate) &&
                candidate != target)
            {
                candidates.Add(candidate);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.Log("No candidates in range.");
            return;
        }
        
        float applicationChance = data.ApplicationChancePercent / 100f;
        if (UnityEngine.Random.value > applicationChance)
        {
            return;
        }
        
        var affected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        _appliedThisTurn.Add(affected);
        affected.GetStatusEffectManager().
            AddStatusEffect(new Burn(data.Duration, Character));
        Debug.Log($"Adding burn to {affected.GetFaction()} {affected.GetCharacterClass()}");
    }
}
