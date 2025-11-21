using System.Collections.Generic;
using UnityEngine;

public abstract class RoundAOEPattern : ScriptableObject
{
    protected int _radius;
    public abstract List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile);

    public void SetRadius(int radius) {
        _radius = radius;
    }
}
