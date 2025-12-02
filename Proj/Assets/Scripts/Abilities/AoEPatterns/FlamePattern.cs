using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlamePattern", menuName = "Scriptable Objects/Abilities/Patterns/FlamePattern")]
public class FlamePattern : DirectedAOEPattern
{
    private int _totalLength = 4;

    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        return CalculateFlamePattern(targetTile, GetDirection());
    }

    private List<CombatGridTile> CalculatePatternBasedOnDirection(CombatGridTile targetTile)
    {
        switch (GetDirection())
        {
            case Direction.Up:
                return CalculateFlamePattern(targetTile, Direction.Up);
            case Direction.Down:
                return CalculateFlamePattern(targetTile, Direction.Down);
            case Direction.Left:
                return CalculateFlamePattern(targetTile, Direction.Left);
            case Direction.Right:
                return CalculateFlamePattern(targetTile, Direction.Right);
        }

        Debug.LogError("Direction not set when calculating flame pattern.");
        return null;
    }

    private List<CombatGridTile> CalculateFlamePattern(CombatGridTile targetTile, Direction direction)
    {
        List<CombatGridTile> list = new();
        int xDirection = 0;
        int yDirection = 0;

        switch (GetDirection())
        {
            case Direction.Up: yDirection = 1; break;
            case Direction.Down:yDirection = -1; break;
            case Direction.Left: xDirection = -1; break;
            case Direction.Right: xDirection = 1; break;
        }

        Vector2Int currentTileCoordinate = targetTile.GetTileIndex();
        for (int i = 1; i<_totalLength; i++)
        {
            var tileObj = CombatGrid._instance.GetTileAtCoord(currentTileCoordinate.x, currentTileCoordinate.y);

            if (!OutOfBounds(currentTileCoordinate) && tileObj.TryGetComponent<CombatGridTile>(out var tile))
            {
                if (!OutOfBounds(currentTileCoordinate))
                {
                    list.Add(tile);
                }
            }
            currentTileCoordinate.x += xDirection;
            currentTileCoordinate.y += yDirection;
        }
        return list;
    }


}
