using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RC_NonBlock", menuName = "Scriptable Objects/Abilities/Range Calculations/Non-Blocking")]

public class RC_NonBlock : RangeCalculation
{
    public override List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range)
    {
        List<GameObject> objects = GridExplorer._instance.GetTilesInRange(tile.gameObject, range, false);
        return ConvertToGridTiles(objects);
    }
}
