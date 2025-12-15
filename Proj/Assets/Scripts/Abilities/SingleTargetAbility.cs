using System.Collections.Generic;
using UnityEngine;

public abstract class SingleTargetAbility : Ability
{
    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        ApplyEffectOnTile(casterTile, targetTile);
    }
    public override void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        PreviewEffectOnTile(casterTile, targetTile);
    }

    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile targetTile)
    {
        if (targetTile == null)
            return null;

        // Get caster
        Character caster = GetAbilityHandler().GetCharacterCaster();
        if (caster == null) return null;

        // Check if target tile is in range.
        bool inRange = caster.GetAbilityHandler().GetTilesInRange().Contains(targetTile);
        if (!inRange) return null;

        List<CombatGridTile> TilesToEffect = new();
        TilesToEffect.Add(targetTile);
        return TilesToEffect;
    }
}
