using UnityEngine;

[System.Serializable]
public class TileEntry
{
    public Vector2Int _tileIndex;
    public Vector3 _position;
    public Vector3 _size;
    public TileType _tileType;
    public GameObject _tile;
    public GameObject _occupant;

    public TileEntry() { }
    public TileEntry(Vector3 goPos, Vector3 goSize, GameObject prefab, GameObject parent, Vector2Int gridPos)
    {
        if (prefab != null)
        {
            // GameObject specific
            this._tile = GameObject.Instantiate(prefab);
            this._tile.transform.position = goPos;
            this._tile.transform.localScale = goSize;
            this._tile.GetComponent<CombatGridTile>().SetTileIndex(gridPos);
            if (parent != null)
                this._tile.transform.SetParent(parent.transform);

            // Save/Load Data Specific
            this._tileType = prefab.GetComponent<CombatGridTile>().GetTileType();
            this._tileIndex = gridPos;
            this._position = goPos;
            this._size = goSize;
            this._occupant = null;
        }
    }
};
