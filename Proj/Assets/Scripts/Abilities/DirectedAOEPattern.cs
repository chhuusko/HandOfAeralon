using UnityEngine;

public abstract class DirectedAOEPattern : AOEPattern
{
    private DirectedAOEAbility.Direction _direction;

    public void SetDirection(DirectedAOEAbility.Direction direction)
    {
        _direction = direction;
    }
}