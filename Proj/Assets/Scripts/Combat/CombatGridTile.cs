using UnityEngine;

[System.Serializable]
public enum TileType
{
    Walkable,
    Impassable,
    Lava,
    Poison
};

public class CombatGridTile : MonoBehaviour
{
    [SerializeField] private TileType tileType;
    [SerializeField] private Vector2 position;

    public TileType GetTileType() {  return tileType; } 
}
