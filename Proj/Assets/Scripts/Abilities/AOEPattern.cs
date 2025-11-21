using System.Collections.Generic;
using UnityEngine;

public abstract class AOEPattern : ScriptableObject
{
    public abstract List<CombatGridTile> CalculateTilesToEffect(CombatGridTile targetTile);

    public virtual void SetRadius(int radius) {
        // Only used by patterns that uses radius.
    }
}
