using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterPrefabEntry
{
    public CharacterClass _classType;
    public List<GameObject> _prefabList;
}

[CreateAssetMenu(fileName = "CharacterPrefabLibrary", menuName = "Scriptable Objects/Character Prefab Library")]
public class CharacterPrefabLibrary : ScriptableObject
{
    public List<CharacterPrefabEntry> _characterPrefabs;

    private Dictionary<CharacterClass, List<GameObject>> _dictionary;

    public GameObject GetPrefab(CharacterClass classType)
    {
        if (_dictionary == null)
        {
            _dictionary = new Dictionary<CharacterClass, List<GameObject>>();
            foreach (CharacterPrefabEntry entry in _characterPrefabs)
            {
                List<GameObject> list = _dictionary[entry._classType];
                list.Add(entry._prefabList[0]);
            }
        }

        // TODO (Calle): Wait to loop over the list untill we have a gender! :D
        //foreach(GameObject character in _dictionary[classType])
        //{
        //    if(character.GetComponent<Character>().GetGender() == _dictionary[])
        //}
        List<GameObject> characterListOfClass = _dictionary[classType];
        return characterListOfClass[0];
    }
}
