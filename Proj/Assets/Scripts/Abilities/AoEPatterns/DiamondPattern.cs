using System.Collections.Generic;
using UnityEngine;

public class DiamondPattern : AOEPattern
{
    private int _radius;
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

                    var tile = CombatManager._instance.GetTileComponent(checkX, checkY);
                    if (tile != null)
                        TilesToEffect.Add(tile);
                }
            }
        }

        return TilesToEffect;
    }
    public override void SetRadius(int radius)
    {
        _radius = radius;
    }
}

