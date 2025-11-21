using System.Collections.Generic;
using UnityEngine;

public abstract class AOEPattern : ScriptableObject
{
    public abstract List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile);
}
