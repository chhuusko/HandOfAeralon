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
    [SerializeField] private TileType _tileType;
    [SerializeField] private Vector2 _tileIndex;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _size;
    [SerializeField] private bool bWalkable;

    public CombatGridTileData(TileType tileType, Vector2 tileIndex, Vector3 pos, Vector3 size)
    {
        this._tileType = tileType;
        if (tileType == TileType.Impassable)
            this.bWalkable = false;
        else
            this.bWalkable = true;

        this._tileIndex = tileIndex;
        this._position = pos;
        this._size = size;
    }

    public TileType GetTileType() { return _tileType; }
    public Vector2 GetTileIndex() { return _tileIndex; }
    public Vector3 GetTilePosition() { return _position; }
    public Vector3 GetTileSize() { return _size; }
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

