using System.Collections.Generic;
using UnityEngine;

public abstract class DirectedAOEAbility : AOEAbility
{
    [SerializeField] private Direction _direction;
    public enum Direction
    {
        Up, Down, Left, Right
    }
    public Direction GetDirection => _direction;

    public void SetDirection(Direction direction)
    {
        _direction = direction;
    }

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

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
}
