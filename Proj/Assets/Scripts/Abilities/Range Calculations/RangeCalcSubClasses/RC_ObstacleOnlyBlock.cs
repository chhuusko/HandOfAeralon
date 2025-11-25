using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RC_ObstacleBlock", menuName = "Scriptable Objects/Abilities/Range Calculations/Obstacle Block")]
public class RC_ObstacleBlock : RangeCalculation
{
    [SerializeField] private float _lineOfSightHeight = 1f;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] LayerMask obstacleLayer;

    /// <summary>
    /// Calculates all tiles within range that have a clear line of sight from the caster tile.
    /// Uses GridExplorer for base range and filters out tiles blocked by obstacles.
    /// </summary>
    /// <param name="tile">The tile the range is calculated from.</param>
    /// <param name="range">Maximum range in tiles.</param>
    /// <returns>List of CombatGridTiles that are reachable and not blocked.</returns>
    public override List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range)
    {
        // Gets all tiles in range from Grid Explorer.
        List<GameObject> objects = GridExplorer._instance.GetTilesInRange(tile.gameObject, range, false);
        // Filteres out tile objects with other objects in LOS.
        List<GameObject> filteredObjects = FilterBlockedObjects(tile.gameObject, objects);
        // Returns CombatGridTile instances of the tile objects.
        return ConvertToGridTiles(filteredObjects);
    }

    /// <summary>
    /// Filters out target tiles that have their line of sight blocked by obstacles.
    /// </summary>
    /// <param name="casterTile">The tile the ability originates from.</param>
    /// <param name="targets">All potential target tiles in range.</param>
    /// <returns>List of GameObjects with clear line of sight.</returns>
    private List<GameObject> FilterBlockedObjects(GameObject casterTile, List<GameObject> targets)
    {
        List<GameObject> filteredList = new();

        // Goes through all tile objects in range and check's if there are other objects blocking.
        foreach(GameObject target in targets){
            if (CheckLineOfSight(casterTile, target)) filteredList.Add(target);
        }
        return filteredList;
    }

    /// <summary>
    /// Checks if there is a clear line of sight between two tiles using a SphereCast.
    /// Returns true when no obstacles are blocking the path.
    /// </summary>
    /// <param name="casterTile">Origin tile.</param>
    /// <param name="targetTile">Target tile.</param>
    /// <returns>True if line of sight is unobstructed.</returns>
    private bool CheckLineOfSight(GameObject casterTile, GameObject targetTile)
    {
        // Get start and end position from tiles.
        Vector3 startPosition = casterTile.transform.position + Vector3.up * _lineOfSightHeight;
        Vector3 endPosition = targetTile.transform.position + Vector3.up * _lineOfSightHeight;

        // Normalize direction and get distance between tiles.
        Vector3 direction = (endPosition - startPosition).normalized;
        float distance = Vector3.Distance(startPosition, endPosition);

        // Debug sphere cast.
        Debug.DrawLine(startPosition, endPosition, Color.red, 1f);


        // Return true if there are no blocking obstacles.
        return !Physics.SphereCast(startPosition, _radius, direction, out RaycastHit hit, distance, obstacleLayer);
    }
}
