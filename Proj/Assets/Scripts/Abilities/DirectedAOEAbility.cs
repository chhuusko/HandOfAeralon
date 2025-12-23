using System.Collections.Generic;
using UnityEngine;

public abstract class DirectedAOEAbility : AOEAbility
{
    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile targetTile)
    {
        // Works like the base version of GetTilesToEffect but only returns the list when valid target is hovered. 
        // Also removes caster tile as target. 

        if (targetTile == null)
            return null;

        // Get caster
        Character caster = GetAbilityHandler().GetCharacterCaster();
        if (caster == null) return null;

        // Check if ability can be cast on target tile.
        bool canCast = caster.GetAbilityHandler().CanCastAbility(this, targetTile);
        if (!canCast) return null;

        var tile = caster.GetCurrentTileComponent();

        if (tile == null) return null;

        var directedAOEPattern = _pattern as DirectedAOEPattern;

        if (directedAOEPattern == null)
        {
            Debug.LogError("Pattern is not a DirectedAOEPattern");
            return null;
        }

        directedAOEPattern.SetDirection(CalculateDirection(tile, targetTile));
        directedAOEPattern.SetCasterTile(tile);

        // Calculate which tiles to effect.
        var list = _pattern.CalculateTilesToEffect(targetTile);

        // Remove caster tile. Unnecessary if pattern already removes caster.
        if (caster.GetCurrentTileComponent())
        {
            list.Remove(caster.GetCurrentTileComponent());
        }

        return list;
    }

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around within pattern and apply effect to all of them.

        var directedAOEPattern = _pattern as DirectedAOEPattern;

        if (directedAOEPattern == null)
        {
            Debug.LogError("Pattern is not a DirectedAOEPattern");
            return;
        }

        directedAOEPattern.SetDirection(CalculateDirection(casterTile, targetTile));
        directedAOEPattern.SetCasterTile(casterTile);

        List<CombatGridTile> tilesToEffect = directedAOEPattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                ApplyEffectOnTile(casterTile, tile);
            }
        }
    }

    public override void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around within pattern and apply effect to all of them.

        var directedAOEPattern = _pattern as DirectedAOEPattern;

        if (directedAOEPattern == null)
        {
            Debug.LogError("Pattern is not a DirectedAOEPattern");
            return;
        }

        directedAOEPattern.SetDirection(CalculateDirection(casterTile, targetTile));
        directedAOEPattern.SetCasterTile(casterTile);

        List<CombatGridTile> tilesToEffect = directedAOEPattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                PreviewEffectOnTile(casterTile, tile);
                var character = tile.GetOccupantCharacter();
                if (character == null) continue;
                character.ShowPreviewVFX();
                GetAbilityHandler().AddPreviewedCharacter(character);
            }
        }
    }

    protected DirectedAOEPattern.Direction CalculateDirection(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate direction based of index of caster and target tile.

        Vector2Int targetIndex = targetTile.GetTileIndex();
        Vector2Int casterIndex = casterTile.GetTileIndex();

        Vector2Int compareIndex = targetIndex - casterIndex;

        switch (compareIndex.x, compareIndex.y)
        {
            case (0, 1): return DirectedAOEPattern.Direction.Up;
            case (0, -1): return DirectedAOEPattern.Direction.Down;
            case (-1, 0): return DirectedAOEPattern.Direction.Left;
            case (1, 0): return DirectedAOEPattern.Direction.Right;
            default:
                {
                    Debug.LogError("Failed to find ability direction.");
                    return DirectedAOEPattern.Direction.Up;
                }
        }
    }
}
