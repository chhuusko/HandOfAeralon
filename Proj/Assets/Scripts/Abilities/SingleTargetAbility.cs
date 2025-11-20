using System.Collections.Generic;
using UnityEngine;

public abstract class SingleTargetAbility : Ability
{
    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        ApplyEffectOnTile(casterTile, targetTile);
    }

    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile tile)
    {
        List<CombatGridTile> TilesToEffect = new();
        TilesToEffect.Add(tile);
        return TilesToEffect;
    }

}
