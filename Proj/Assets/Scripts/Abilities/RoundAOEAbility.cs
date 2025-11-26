using UnityEngine;
using System.Collections.Generic;

public abstract class RoundAOEAbility : AOEAbility
{
    [Header("- Type Specific values - ")]
    [SerializeField] protected int _radius;
    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                ApplyEffectOnTile(casterTile, tile);
            }
        }
    }
    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile tile)
    {
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        return _pattern.CalculateTilesToEffect(tile);
    }
}
