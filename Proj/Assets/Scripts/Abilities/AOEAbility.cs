using UnityEngine;
using System.Collections.Generic;

public abstract class AOEAbility : Ability
{
    [Header("- Type Specific values - ")]
    [SerializeField] private AOEPattern pattern;
    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.

        List<CombatGridTile> tilesToEffect = pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                ApplyEffectOnTile(tile);
            }
        }
    }
    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile tile)
    {
        return pattern.CalculateTilesToEffect(tile);
    }
}
