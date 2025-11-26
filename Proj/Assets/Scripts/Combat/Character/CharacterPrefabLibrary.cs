using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterPrefabEntry
{
    public CharacterClass _classType;
    public GameObject _prefab;
    //public List<GameObject> _prefabList; // NOTE (Calle): For multiple models
}

[CreateAssetMenu(fileName = "CharacterPrefabLibrary", menuName = "Resources/Character Prefab Library")]
public class CharacterPrefabLibrary : ScriptableObject
{
    public List<CharacterPrefabEntry> _characterPrefabs;

    //private Dictionary<CharacterClass, List<GameObject>> _dictionary; // Note (Calle): For Multiple Models
    private Dictionary<CharacterClass, GameObject> _dictionary;

    public GameObject GetPrefab(CharacterClass classType)
    {

        // NOTE (Calle): Use this if there are more different models to load based on BodyType or something else besides ClassType
        //if (_dictionary == null)
        //{
        //    _dictionary = new Dictionary<CharacterClass, List<GameObject>>();
        //    foreach (CharacterPrefabEntry entry in _characterPrefabs)
        //    {
        //        List<GameObject> list = _dictionary[entry._classType];
        //        list.Add(entry._prefabList[0]);
        //    }
        //}

        // TODO (Calle): Loop over the list based on secondary model identification! :D
        //foreach(GameObject character in _dictionary[classType])
        //{
        //    if(character.GetComponent<Character>().GetGender() == _dictionary[])
        //}

        if(_dictionary == null)
        {
            _dictionary = new Dictionary<CharacterClass, GameObject>();
            foreach (CharacterPrefabEntry characterEntry in _characterPrefabs)
            {
                _dictionary[characterEntry._classType] = characterEntry._prefab;
            }
        }

        // NOTE (Calle): For multiple models
        //GameObject characterListOfClass = _dictionary[classType];
        //return characterListOfClass[0];

        return _dictionary[classType];
    }
}
