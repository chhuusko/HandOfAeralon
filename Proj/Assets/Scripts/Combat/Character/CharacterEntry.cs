using UnityEngine;


[System.Serializable]
public class CharacterEntry
{
    public Vector3 _position;
    public Vector3 _size;
    public Quaternion _rotation;
    public Vector2Int _tileIndex;

    public CharacterClass _characterClass;
    public int _baseHealthPoints;
    public int _baseSpeed;
    public int _baseDamage;
    public int _baseMovementPoints;
    public int _currentHealthPoints;
    public int _currentSpeed;
    public int _currentDamage;
    public int _currentMovementPoints;
    public GameObject _character;

    public CharacterEntry() { }
    public CharacterEntry(Vector3 goPos, Vector3 goSize, Quaternion rotation, GameObject prefab, GameObject parent, Vector2Int gridPos)
    {
        if (prefab != null)
        {
            // GameObject Specific
            this._character = GameObject.Instantiate(prefab);
            this._character.transform.position = goPos;
            this._character.transform.localScale = goSize;
            this._character.transform.rotation = rotation;
            this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);
            this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);
            this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);
            this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);


            if (parent != null)
                this._character.transform.SetParent(parent.transform);



            // Save/Load Specific
            this._characterClass = prefab.GetComponent<Character>().GetCharacterClass();
            this._tileIndex = gridPos;
            this._position = goPos;
            this._size = goSize;
        }
    }

    public CharacterEntry(CombatGridCharacterData characterData, GameObject prefab, GameObject parent)
    {
        if (prefab != null)
        {
            // GameObject Specific
            this._character = GameObject.Instantiate(prefab);
            this._character.transform.position = characterData.GetCharacterPosition();
            this._character.transform.localScale = Vector3.one;
            this._character.transform.rotation = characterData.GetRotation();

            this._character.GetComponent<Character>().SetCurrentTileIndex(characterData.GetTileIndex());
            this._character.GetComponent<Character>().SetBaseHealthPoints(characterData.GetHealthPoints());
            //this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);
            //this._character.GetComponent<Character>().SetCurrentTileIndex(gridPos);


            if (parent != null)
                this._character.transform.SetParent(parent.transform);


            // Save/Load Specific
            this._characterClass = prefab.GetComponent<Character>().GetCharacterClass();
            this._tileIndex = characterData.GetTileIndex();
            this._position = characterData.GetCharacterPosition();
            this._size = Vector3.one;
        }
    }

};
