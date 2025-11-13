using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AOEAbility", menuName = "Scriptable Objects/Abilities/AOE")]
public abstract class AOEAbility : Ability
{
    [Header("- Type Specific values - ")]
    [SerializeField] private int _radius = 1;

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.

        List<CombatGridTile> tilesToEffect = CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                ApplyEffectOnTile(tile);
            }
        }
    }

    private List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile)
    {
        List<CombatGridTile> tilesToEffect = new List<CombatGridTile>();
        Vector2 targetIndex = targetTile.GetTileIndex();
        int sideToSideLength = _radius * 2;

        // Loop around target tile in a square and add to "tiles to effect".
        for (int x = 0; x <= sideToSideLength; x++)
        {
            for (int y = 0; y <= sideToSideLength; y++)
            {
                int offsetX = x - _radius;
                int offsetY = y - _radius;

                int checkX = (int)targetIndex.x + offsetX;
                int checkY = (int)targetIndex.y + offsetY;

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
