using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HousePattern", menuName = "Scriptable Objects/Abilities/Patterns/HousePattern")]

public class HousePattern : DirectedAOEPattern
{
    private int _size = 3;
    
    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        switch (_direction)
        {
            case Direction.Left: return CalculateSquareWithoutCaster(CalculateTopRightTile(0, 1, targetTile));
            case Direction.Right: return CalculateSquareWithoutCaster(CalculateTopRightTile(2, 1, targetTile));
            case Direction.Up: return CalculateSquareWithoutCaster(CalculateTopRightTile(1, 2, targetTile));
            case Direction.Down: return CalculateSquareWithoutCaster(CalculateTopRightTile(1, 0, targetTile));
            default: 
                {
                    Debug.LogError("Something went wrong when calculating housePattern for an ability.");
                    return null;
                }
        }
    }

    private CombatGridTile CalculateTopRightTile(int plusX, int plusY, CombatGridTile targetTile)
    {
        Vector2Int startIndex = targetTile.GetTileIndex();
        startIndex.x += plusX;
        startIndex.y += plusY;
        GameObject tileObj = CombatGrid._instance.GetTileAtCoord(startIndex.x, startIndex.y);
        if (tileObj == null) return null;
        CombatGridTile tile = tileObj.GetComponent<CombatGridTile>();
        if (tile == null) return null;
        return tile;
    }

    private List<CombatGridTile> CalculateSquareWithoutCaster(CombatGridTile topRightTileOfSquare)
    {
        List<CombatGridTile> squareWithoutCaster = new();
        Vector2Int startIndex = topRightTileOfSquare.GetTileIndex();

        for (int i = startIndex.x; startIndex.x >= startIndex.x - _size; i--)
        {
            for (int j = startIndex.y; startIndex.y >= startIndex.y - _size; j--)
            {
                GameObject tileObj = CombatGrid._instance.GetTileAtCoord(i, j);
                if(tileObj == null) continue;
                CombatGridTile tile = tileObj.GetComponent<CombatGridTile>();
                if(tile == null) continue;
                if(tile == _casterTile) continue;
                squareWithoutCaster.Add(tile);
            }
        }
        return squareWithoutCaster;
    }
}
