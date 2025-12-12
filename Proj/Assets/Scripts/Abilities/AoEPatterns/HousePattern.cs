using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HousePattern", menuName = "Scriptable Objects/Abilities/Patterns/HousePattern")]

public class HousePattern : DirectedAOEPattern
{
    private int _size = 3;
    
    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        Vector2Int targetIndex = targetTile.GetTileIndex();
       return CalculateSquareWithoutCaster(CalculateTopRightTile(1, 1, targetIndex));
    }

    private Vector2Int CalculateTopRightTile(int plusX, int plusY, Vector2Int targetIndex)
    {
        Vector2Int startIndex = targetIndex;
        startIndex.x += plusX;
        startIndex.y += plusY;
       
        return startIndex;
    }

    private List<CombatGridTile> CalculateSquareWithoutCaster(Vector2Int topRightTileIndexOfSquare)
    {
        List<CombatGridTile> squareWithoutCaster = new();
        Vector2Int currentIndex = topRightTileIndexOfSquare;

        for (int x = currentIndex.x; x > currentIndex.x - _size; x--)
        {
            for (int y = currentIndex.y; y > currentIndex.y - _size; y--)
            {
                if (OutOfBounds(new Vector2Int(x, y))) continue;

                GameObject tileObj = CombatGrid._instance.GetTileAtCoord(x, y);
                if(tileObj == null) continue;
                CombatGridTile tile = tileObj.GetComponent<CombatGridTile>();
                if (!tile || tile == _casterTile) continue;
                squareWithoutCaster.Add(tile);
            }
        }
        return squareWithoutCaster;
    }
}
