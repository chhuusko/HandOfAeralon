using UnityEngine;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public class RC_NonBlock : RangeCalculation
{
    public override List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range)
    {
        List<GameObject> objects = GridExplorer._instance.GetTilesInRange(tile.gameObject, range, false);
        return ConvertToGridTiles(objects);
    }
}
