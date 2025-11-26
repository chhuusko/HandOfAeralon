using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[System.Serializable]
public enum TileType
{
    UnInitialized,
    Walkable,
    Impassable,
    Lava,
    Poison,
    Deploy
};

[System.Serializable]
public class CombatGridTileData
{
    [SerializeField] private TileType   _tileType;
    [SerializeField] private Vector2Int _tileIndex;
    [SerializeField] private Vector3    _position;
    [SerializeField] private Vector3    _size;
    [SerializeField] private bool       _bWalkable;

    public CombatGridTileData(TileType tileType, 
                              Vector2Int tileIndex, 
                              Vector3 pos, 
                              Vector3 size)
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

    public CombatGridTileData(TileEntry tileEntry)
    {
        _tileType = tileEntry._tileType;

        if (tileEntry._tileType == TileType.Impassable)
            _bWalkable = false;
        else
            _bWalkable = true;

        _tileIndex = tileEntry._tileIndex;
        _position = tileEntry._position;
        _size = tileEntry._size;
    }

    public TileType GetTileType() { return _tileType; }
    public Vector2Int GetTileIndex() { return _tileIndex; }
    public Vector3 GetTilePosition() { return _position; }
    public Vector3 GetTileSize() { return _size; }
    public bool IsWalkable() { return _bWalkable; }
    public void SetTilePosition(Vector3 position) { _position = position; }
    public void SetWalkable(bool bWalkable) { this._bWalkable = bWalkable; }
    public void SetTileIndex(Vector2Int tileIndex) { this._tileIndex = tileIndex; }
    public void SetTileType(TileType tileType) { this._tileType = tileType; }
};




