using UnityEditor.Build;
using UnityEngine;

public abstract class DirectedAOEPattern : AOEPattern
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    protected Direction _direction;
    protected CombatGridTile _casterTile;

    public Direction GetDirection() => _direction;
    public void SetDirection(Direction direction)
    {
        _direction = direction;
    }

    public void SetCasterTile(CombatGridTile tile)
    {
        _casterTile = tile;
    }
}