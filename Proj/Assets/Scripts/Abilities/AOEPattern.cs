using System.Collections.Generic;
using UnityEngine;

public abstract class AOEPattern : ScriptableObject
{
    public abstract List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile);

    protected bool OutOfBounds(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= CombatGrid._instance.GetGridWidth() || pos.y >= CombatGrid._instance.GetGridHeight())
        {
            return true;
        }

        return false;
    }
}
