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
    [SerializeField] private TileType   _tileType;
    [SerializeField] private Vector2Int _tileIndex;
    [SerializeField] private Vector3    _position;
    [SerializeField] private Vector3    _size;
    [SerializeField] private bool       _bWalkable;

    public CombatGridTileData(TileType tileType, Vector2Int tileIndex, Vector3 pos, Vector3 size)
    {
        this._tileType = tileType;
        if (tileType == TileType.Impassable)
            this._bWalkable = false;
        else
            this._bWalkable = true;

        this._tileIndex = tileIndex;
        this._position = pos;
        this._size = size;
    }

    public TileType GetTileType() { return _tileType; }
    public Vector2Int GetTileIndex() { return _tileIndex; }
    public Vector3 GetTilePosition() { return _position; }
    public Vector3 GetTileSize() { return _size; }
    public bool IsWalkable() { return _bWalkable; }
    public void SetWalkable(bool bWalkable) { this._bWalkable = bWalkable; }
    public void SetTileIndex(Vector2Int tileIndex) { this._tileIndex = tileIndex; }
    public void SetTileType(TileType tileType) { this._tileType = tileType; }
};


// NOTE (Calle): This is for saving a CombatGrid to JSON in the GridEditor
[System.Serializable]
public class CombatGridSerializedSaveData
{
    public int _gridWidth;
    public int _gridHeight;
    public Vector3 _tileSize;
    public List<CombatGridTileData> _tileData = new List<CombatGridTileData>();
}

