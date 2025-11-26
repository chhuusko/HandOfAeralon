using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RC_MeleeNonBlock", menuName = "Scriptable Objects/Abilities/Range Calculations/Melee-Non-Blocking")]

public class RC_MeleeNonBlock : RangeCalculation
{
    public override List<CombatGridTile> CalculateTilesInRange(CombatGridTile tile, int range)
    {
        List<CombatGridTile> tiles = GridExplorer._instance.GetTilesInMeleeRange(tile.gameObject);
        return tiles;
    }
}
