using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriSquarePattern", menuName = "Scriptable Objects/Abilities/Patterns/TriSquarePattern")]
public class TriSquarePattern : DirectedAOEPattern
{
    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        return CalculateLinePattern(targetTile, GetDirection());
    }

    private List<CombatGridTile> CalculateLinePattern(CombatGridTile targetTile, Direction direction)
    {
        List<CombatGridTile> list = new();
        int xDirection;

        switch (GetDirection())
        {
            case Direction.Left: xDirection = -1; break;
            case Direction.Right: xDirection = 1; break;
            default: xDirection = 0; break;
        }

        Vector2Int centerTileCoordinate = targetTile.GetTileIndex();
        AddTileToList(centerTileCoordinate.x, centerTileCoordinate.y, list);
        
        if(xDirection != 0)
        {
            AddTileToList(centerTileCoordinate.x, centerTileCoordinate.y + 1, list);
            AddTileToList(centerTileCoordinate.x, centerTileCoordinate.y - 1, list);
        }
        else
        {
            AddTileToList(centerTileCoordinate.x +1, centerTileCoordinate.y, list);
            AddTileToList(centerTileCoordinate.x -1, centerTileCoordinate.y, list);
        }
        return list;
    }

    private void AddTileToList(int x, int y, List<CombatGridTile> list)
    {
        Vector2Int coord = new Vector2Int(x, y);
        var tileObject = CombatGrid._instance.GetTileAtCoord(x, y);
        if (!OutOfBounds(coord) && tileObject.TryGetComponent<CombatGridTile>(out var tile))
        {
            list.Add(tile);
        }
    }
}
