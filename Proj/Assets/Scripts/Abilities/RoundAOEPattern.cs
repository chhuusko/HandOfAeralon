using System.Collections.Generic;
using UnityEngine;

public abstract class RoundAOEPattern : AOEPattern
{
    protected int _radius;

    public void SetRadius(int radius) {
        _radius = radius;
    }

    public int GetRadius() => _radius;
}
