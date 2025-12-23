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
        var character = targetTile.GetOccupantCharacter();
        if (character == null) return;
        character.ShowPreviewVFX();
    }

    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile targetTile)
    {
        if (targetTile == null) return null;

        var handler = GetAbilityHandler();
        if (handler == null) return null;

        if (!handler.GetTilesInRange().Contains(targetTile))
            return null;

        return new List<CombatGridTile> { targetTile };
    }
}
