using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiamondPattern", menuName = "Scriptable Objects/Abilities/Patterns/DiamondPattern")]
public class DiamondPattern : RoundAOEPattern
{
    public override List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        // Calculates every tile around target in every direction based on radius, but radius counts as 2.

        List<CombatGridTile> TilesToEffect = new();
        Vector2 centerIndex = targetTile.GetTileIndex();

        // Loop around target tile in a diamond and add to "tiles to effect".
        for (int x = -_radius; x <= _radius; x++)
        {
            for (int y = -_radius; y <= _radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= _radius)
                {
                    int checkX = (int) centerIndex.x + x;
                    int checkY = (int) centerIndex.y + y;

                    if (checkX < 0 || checkX >= CombatGrid._instance.GetGridWidth()) continue;
                    if (checkY < 0 || checkY >= CombatGrid._instance.GetGridHeight()) continue;

                    var tile = CombatManager._instance.GetTileComponent(checkX, checkY);
                    if (tile != null)
                        TilesToEffect.Add(tile);
                }
            }
        }

        return TilesToEffect;
    }
  
}

