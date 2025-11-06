using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TilePrefabEntry
{
    public TileType type;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "TilePrefabLibrary", menuName = "Tiles/Tile Prefab Library")]
public class TilePrefabLibrary : ScriptableObject
{
    public List<TilePrefabEntry> tilePrefabs;

    private Dictionary<TileType, GameObject> _dictionary;

    public GameObject GetPrefab(TileType type)
    {
        if (_dictionary == null)
        {
            _dictionary = new Dictionary<TileType, GameObject>();
            foreach (var entry in tilePrefabs)
            {
                _dictionary[entry.type] = entry.prefab;
            }
        }

        return _dictionary[type];
    }
}

