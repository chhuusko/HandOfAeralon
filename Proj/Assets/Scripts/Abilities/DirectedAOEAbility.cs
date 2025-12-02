using System.Collections.Generic;
using UnityEngine;

public abstract class DirectedAOEAbility : AOEAbility
{
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
