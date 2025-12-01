using UnityEngine;

public abstract class DirectedAOEPattern : AOEPattern
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    private Direction _direction;

    public Direction GetDirection => _direction;

    public void SetDirection(Direction direction)
    {
        _direction = direction;
    }
}
