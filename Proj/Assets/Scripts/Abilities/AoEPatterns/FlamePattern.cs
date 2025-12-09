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

    private List<CombatGridTile> CalculateFlamePattern(CombatGridTile targetTile, Direction direction)
    {
        // Calculates the whole flame pattern by using a direction to add tiles in a straight line in that direction.
        // Uses help method AddTilesOnSides to add extra tiles on both sides of the line.

        // Declare empty variables.
        List<CombatGridTile> list = new();
        int xDirection = 0;
        int yDirection = 0;

        // Get direction.
        switch (GetDirection())
        {
            case Direction.Up: yDirection = 1; break;
            case Direction.Down: yDirection = -1; break;
            case Direction.Left: xDirection = -1; break;
            case Direction.Right: xDirection = 1; break;
        }

        // Add Tiles in a line.
        Vector2Int currentTileCoordinate = targetTile.GetTileIndex();
        for (int i = 0; i < _totalLength; i++)
        {
            var tileObj = CombatGrid._instance.GetTileAtCoord(currentTileCoordinate.x, currentTileCoordinate.y);

            if (!OutOfBounds(currentTileCoordinate) && tileObj.TryGetComponent<CombatGridTile>(out var tile))
            {
                list.Add(tile);
                AddTilesOnSides(xDirection, tile, list, i);
            }
            // Based on the direction, the currentTileCoordinate's X or Y is updated.
            currentTileCoordinate.x += xDirection;
            currentTileCoordinate.y += yDirection;
        }
        return list;
    }

    private void AddTilesOnSides(int xDirection, CombatGridTile centerTile, List<CombatGridTile> list, int currentIndex)
    {
        // Adds Tiles on sides based on direction.
        if (xDirection != 0)
        {
            // If direction is to a side, add tiles over and under.
            AddSideLine(centerTile, Vector2Int.up, currentIndex, list);
            AddSideLine(centerTile, Vector2Int.down, currentIndex, list);
        }
        else
        {
            // If direction is to a over or under, add tiles to both sides.
            AddSideLine(centerTile, Vector2Int.right, currentIndex, list);
            AddSideLine(centerTile, Vector2Int.left, currentIndex, list);
        }
    }
    private void AddSideLine(CombatGridTile centerTile, Vector2Int dir, int currentIndex, List<CombatGridTile> list)
    {
        int _indexWhenExpandingStops = 2;
        // Updates how many tiles should be added on either side based on the current index of the straight line. 

        // Keep expanding pattern til a certain point. Then just keep adding tiles with the same width.
        int maxWidth = Mathf.Min(currentIndex, _indexWhenExpandingStops);
        Debug.Log(_indexWhenExpandingStops);
        for (int i = 0; i <= maxWidth; i++)
        {
            // Calculate tile to the side of center with loop index and direction.
            Vector2Int sideTileIndex = centerTile.GetTileIndex() + dir * i;

            // Don't add if tile is out of bounds.
            if (OutOfBounds(sideTileIndex))
            {
                continue;
            }

            var tileObj = CombatGrid._instance.GetTileAtCoord(sideTileIndex.x, sideTileIndex.y);
            if (tileObj == null) continue;

            if (tileObj.TryGetComponent<CombatGridTile>(out var tile))
            {
                // Add tile at new coordinate if tile is not already in the list.
                if (list.Contains(tile)) continue;
                list.Add(tile);
            }
        }
    }
}
