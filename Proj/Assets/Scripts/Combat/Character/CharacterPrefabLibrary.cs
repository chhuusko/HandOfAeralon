using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterPrefabEntry
{
    public CharacterClass _classType;
    public GameObject _prefab;
    public List<GameObject> _prefabList; // NOTE (Calle): For multiple models
}

[CreateAssetMenu(fileName = "CharacterPrefabLibrary", menuName = "Resources/Character Prefab Library")]
public class CharacterPrefabLibrary : ScriptableObject
{
    public List<CharacterPrefabEntry> _characterPrefabs;

    private Dictionary<CharacterClass, List<GameObject>> _dictionaryMultiple; // Note (Calle): For Multiple Models
    private Dictionary<CharacterClass, GameObject> _dictionary;

    public GameObject GetPrefab(CharacterClass classType)
    {

        if(_dictionary == null)
        {
            _dictionary = new Dictionary<CharacterClass, GameObject>();
            foreach (CharacterPrefabEntry characterEntry in _characterPrefabs)
            {
                _dictionary[characterEntry._classType] = characterEntry._prefab;
            }
        }

        return _dictionary[classType];
    }

    public GameObject GetPrefab(CharacterClass classType, Faction faction)
    {
        // NOTE (Calle): Use this if there are more different models to load based on BodyType or something else besides ClassType
        if (_dictionaryMultiple == null)
        {
            _dictionaryMultiple = new Dictionary<CharacterClass, List<GameObject>>();
            foreach (CharacterPrefabEntry entry in _characterPrefabs)
            {
                if(!_dictionaryMultiple.ContainsKey(entry._classType))
                {
                    _dictionaryMultiple.Add(entry._classType, new List<GameObject>());
                    foreach (GameObject prefab in entry._prefabList)
                    {
                        if (!_dictionaryMultiple[entry._classType].Contains(prefab))
                            _dictionaryMultiple[entry._classType].Add(prefab);
                    }
                }
                else
                {
                    _dictionaryMultiple[entry._classType].Add(entry._prefabList[0]);
                }
            }
        }

        // TODO (Calle): Loop over the list based on secondary model identification! :D
        foreach(GameObject character in _dictionaryMultiple[classType])
        {
            if (character.GetComponent<Character>().GetFaction() == faction)
                return character;
        }

        return null;
    }
}
