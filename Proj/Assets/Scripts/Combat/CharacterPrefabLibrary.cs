using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterPrefabEntry
{
    public CharacterClass classType;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "CharacterPrefabLibrary", menuName = "Scriptable Objects/Character Prefab Library")]
public class CharacterPrefabLibrary : ScriptableObject
{
    public List<CharacterPrefabEntry> _characterPrefabs;

    private Dictionary<CharacterClass, GameObject> _dictionary;

    public GameObject GetPrefab(CharacterClass classType)
    {
        if (_dictionary == null)
        {
            _dictionary = new Dictionary<CharacterClass, GameObject>();
            foreach (var entry in _characterPrefabs)
            {
                _dictionary[entry.classType] = entry.prefab;
            }
        }

        return _dictionary[classType];
    }
}
