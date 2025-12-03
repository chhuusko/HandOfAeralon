using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinePattern", menuName = "Scriptable Objects/Abilities/Patterns/LinePattern")]
public class LinePattern : DirectedAOEPattern
{
    [SerializeField] private int _totalLength = 6;

    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        return CalculateLinePattern(targetTile, GetDirection());
    }

    private List<CombatGridTile> CalculateLinePattern(CombatGridTile targetTile, Direction direction)
    {
        List<CombatGridTile> list = new();
        int xDirection = 0;
        int yDirection = 0;

        switch (GetDirection())
        {
            case Direction.Up: yDirection = 1; break;
            case Direction.Down: yDirection = -1; break;
            case Direction.Left: xDirection = -1; break;
            case Direction.Right: xDirection = 1; break;
        }

        Vector2Int currentTileCoordinate = targetTile.GetTileIndex();
        for (int i = 0; i < _totalLength; i++)
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