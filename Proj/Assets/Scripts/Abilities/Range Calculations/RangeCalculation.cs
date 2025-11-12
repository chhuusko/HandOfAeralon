using UnityEngine;
using System.Collections.Generic;

public abstract class RangeCalculation : ScriptableObject
{
    public abstract List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range);

    protected List<CombatGridTile> ConvertToGridTiles(List<GameObject> tileObjects)
    {
        List<CombatGridTile> combatGridTiles = new List<CombatGridTile>();

        foreach (GameObject tileObject in tileObjects) 
        {
            if(tileObject.TryGetComponent<CombatGridTile>(out var gridTile))
            {
                combatGridTiles.Add(gridTile);
            }
        }
        return combatGridTiles;
    }
}
