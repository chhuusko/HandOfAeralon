using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RC_ObstacleSelfBlock", menuName = "Scriptable Objects/Abilities/Range Calculations/ObstacleSelfBlock")]

public class RC_Obstacle_Self_Block : RC_ObstacleBlock
{
    public override List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range)
    {
        // Gets all tiles in range from Grid Explorer.
        List<GameObject> objects = GridExplorer._instance.GetTilesInRange(tile.gameObject, range, false);

        // Filteres out tile objects with other objects in LOS.
        List<GameObject> filteredObjects = FilterBlockedObjects(tile.gameObject, objects);
        // Returns CombatGridTile instances of the tile objects.
        filteredObjects.Remove(tile.gameObject);
        return ConvertToGridTiles(filteredObjects);
    }
}
