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
    [SerializeField] private bool bWalkable;

    public CombatGridTileData(TileType tileType, Vector2 position)
    {
        this.tileType = tileType;
        if (tileType == TileType.Impassable)
            this.bWalkable = false;
        else
            this.bWalkable = true;

        this.position = position;
    }

    public TileType GetTileType() { return tileType; }
    public Vector2 GetTilePosition() { return position; }
    public bool IsWalkable() { return bWalkable; }
    public void SetWalkable(bool bWalkable) { this.bWalkable = bWalkable; }
};


// NOTE (Calle): This is for saving to JSON in the GridEditor
[System.Serializable]
public class CombatGridSerializedSaveData
{
    public int gridWidth;
    public int gridHeight;
    public List<CombatGridTileData> tileData = new List<CombatGridTileData>();
}

