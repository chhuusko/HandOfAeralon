using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquarePattern", menuName = "Scriptable Objects/Abilities/Patterns/SquarePattern")]

public class SquarePattern : RoundAOEPattern
{
    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        // Calculates every tile around target in every direction based on radius.
      
        List<CombatGridTile> tilesToEffect = new();
        Vector2 centerIndex = targetTile.GetTileIndex();
      
        // Loop around target tile in a square and add to "tiles to effect".
        for (int x = -_radius; x <= _radius; x++)
        {
            for (int y = -_radius; y <= _radius; y++)
            {
                int checkX = (int) centerIndex.x + x;
                int checkY = (int) centerIndex.y + y;

                if (checkX < 0 || checkX >= CombatGrid._instance.GetGridWidth()) continue;
                if (checkY < 0 || checkY >= CombatGrid._instance.GetGridHeight()) continue;

                CombatGridTile nearbyTile = CombatManager._instance.GetTileComponent(checkX, checkY);
                if (nearbyTile != null)
                {
                    tilesToEffect.Add(nearbyTile);
                }
            }
        }
        return tilesToEffect;
    }
}
