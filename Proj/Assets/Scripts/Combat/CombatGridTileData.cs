using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TileType
{
    Walkable,
    Impassable,
    Lava,
    Poison
};

[System.Serializable]
public class CombatGridTileData
{
    [SerializeField] private TileType tileType;
    [SerializeField] private Vector2 position;

    public CombatGridTileData(TileType tileType, Vector2 position)
    {
        this.tileType = tileType;
        this.position = position;
    }

    public TileType GetTileType() { return tileType; }
    public Vector2 GetTilePosition() { return position; }
};


// NOTE (Calle): This is for saving to JSON in the GridEditor
[System.Serializable]
public class CombatGridTileSerializedSaveData
{
    public List<CombatGridTileData> tileData = new List<CombatGridTileData>();
}