using System.Collections.Generic;
using UnityEngine;

public class RC_ObstacleBlock : RangeCalculation
{
    [SerializeField] private float LineOfSightHeight = 1f;
    public override List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range)
    {
        List<GameObject> objects = GridExplorer._instance.GetTilesInRange(tile.gameObject, range, false);
        FilterBlockedObjects(tile.gameObject, objects);
        return ConvertToGridTiles(objects);
    }

    private List<GameObject> FilterBlockedObjects(GameObject casterTile, List<GameObject> targets)
    {
        List<GameObject> filteredList = new();
        foreach(GameObject target in targets){
            if (CheckLineOfSight(casterTile, target)) filteredList.Add(target);
        }
        return filteredList;
    }

    /// <summary>
    /// Returns true if there are no "non-character objects" blocking line of sight.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    /// Returns true if there are no "non-character objects" blocking line of sight.
    private bool CheckLineOfSight(GameObject casterTile, GameObject TargetTile)
    {
        return false;
    }
}
