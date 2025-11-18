using UnityEngine;


[System.Serializable]
public class CharacterEntry
{
    [Header("Character")]
    public CharacterClass _characterClass;
    public Faction _faction;
    
    public Vector3 _position;
    public Vector3 _size;
    public Quaternion _rotation;

    [Header("Base stats")]
    public int _baseHealthPoints;
    public int _baseSpeed;
    public int _baseDamage;
    public int _baseMovementPoints;

    [Header("Current stats")]
    public int _currentHealthPoints;
    public int _currentSpeed;
    public int _currentDamage;
    public int _currentMovementPoints;

    [Header("Misc")]
    public Vector2Int _currentTileIndex;

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
            this._currentTileIndex = gridPos;
            this._position = goPos;
            this._size = goSize;
        }
    }

    public CharacterEntry(CombatGridCharacterData characterData, GameObject prefab, GameObject parent)
    {
        if (characterData == null)
        {
            DebugLog.CJLog("CharacterData was null");
            return;
        }
            

        if (prefab == null)
        {
            DebugLog.CJLog("CharacterPrefab was null");
            return;
        }

        // GameObject Specific
        _character = GameObject.Instantiate(prefab);
        _character.transform.position = characterData.GetCharacterPosition();
        _character.transform.localScale = Vector3.one;
        _character.transform.rotation = characterData.GetRotation();

        _character.GetComponent<Character>().SetCharacterClass(characterData.GetCharacterClass());
        _character.GetComponent<Character>().SetFaction(characterData.GetFaction());

        _character.GetComponent<Character>().SetCurrentHealthPoints(characterData.GetHealthPoints());
        _character.GetComponent<Character>().SetCurrentSpeed(characterData.GetSpeed());
        _character.GetComponent<Character>().SetCurrentDamage(characterData.GetDamage());
        _character.GetComponent<Character>().SetCurrentMovementPoints(characterData.GetMovementPoints());

        _character.GetComponent<Character>().SetBaseHealthPoints(characterData.GetBaseHealthPoints());
        _character.GetComponent<Character>().SetBaseInitiative(characterData.GetBaseSpeed());
        _character.GetComponent<Character>().SetBaseDamage(characterData.GetBaseDamage());
        _character.GetComponent<Character>().SetBaseMovementPoints(characterData.GetBaseMovementPoints());

        _character.GetComponent<Character>().SetCurrentTileIndex(characterData.GetCurrentTileIndex());

        // Save/Load Specific
        _characterClass        = characterData.GetCharacterClass();
        _faction               = characterData.GetFaction();

        _position              = characterData.GetCharacterPosition();
        _size                  = Vector3.one;
        _rotation              = characterData.GetRotation();

        if (parent != null)
            _character.transform.SetParent(parent.transform);

        _baseHealthPoints      = characterData.GetBaseHealthPoints();
        _baseSpeed             = characterData.GetBaseSpeed();
        _baseDamage            = characterData.GetBaseDamage();
        _baseMovementPoints    = characterData.GetBaseMovementPoints();
        
        _currentHealthPoints   = characterData.GetHealthPoints();
        _currentSpeed          = characterData.GetSpeed();
        _currentDamage         = characterData.GetDamage();
        _currentMovementPoints = characterData.GetMovementPoints();
        _currentTileIndex      = characterData.GetCurrentTileIndex();
    }

};
