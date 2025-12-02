using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HousePattern", menuName = "Scriptable Objects/Abilities/Patterns/HousePattern")]

public class HousePattern : DirectedAOEPattern
{
    private int _size = 3;
    
    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
       return CalculateSquareWithoutCaster(CalculateTopRightTile(1, 1, targetTile));
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

        for (int x = startIndex.x; x > startIndex.x - _size; x--)
        {
            for (int y = startIndex.y; y > startIndex.y - _size; y--)
            {
                if (x < 0 || x >= CombatGrid._instance.GetGridWidth()) continue;
                if (y < 0 || y >= CombatGrid._instance.GetGridHeight()) continue;

                GameObject tileObj = CombatGrid._instance.GetTileAtCoord(x, y);
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
