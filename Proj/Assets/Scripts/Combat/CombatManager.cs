using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

[System.Serializable]
public enum CombatState
{
    IntroCinematic,
    LoadCombatLevel,
    PlaceCharacters,
    MakeTurn,
    EndTurn,
    EndCombat
};

[System.Serializable]
public enum CombatTurn
{
    PlayerTurn,
    EnemyTurn
};

[System.Serializable]
public class CombatGrid
{
    [SerializeField] private TilePrefabLibrary      _tilePrefabLibrary;
    [SerializeField] private CharacterPrefabLibrary _characterPrefabLibrary;

    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private Vector3 _tileSize;
    
    [SerializeField] private GameObject[] tilesGO;
    [SerializeField] private List<GameObject> _charactersGO;


    public GameObject[] GetAllTiles() {  return tilesGO; }
    public GameObject GetTileAtCoord(int x, int y) 
    {
        int index = x + y * _width;
        if (index < 0 || index >= _width * _height)
            return null;

        return tilesGO[index];  
    }

    public Vector3 GetTileSize() { return _tileSize; }
    public int GetGridWidth() { return _width; }
    public int GetGridHeight() { return _height; }
    public void SetCombatGridSize(int w, int h)
    {
        _width  = w;
        _height = h;
        tilesGO = new GameObject[w * h];
    }
    public void SetTileSize(Vector3 tileSize)
    {
        _tileSize = tileSize;
    }

    public void AddTile(CombatGridTileData tileData)
    {
        if (tileData.GetTileType() == TileType.UnInitialized)
            return;

        Vector2 tileIndex = tileData.GetTileIndex();
        Vector3 instancePos = tileData.GetTilePosition();

        GameObject tilePrefab = _tilePrefabLibrary.GetPrefab(tileData.GetTileType());
        GameObject tileObject = Object.Instantiate(tilePrefab, instancePos, Quaternion.identity);

        tileObject.transform.localScale = tileData.GetTileSize();
        tileObject.GetComponent<CombatGridTile>().SetTileType(tileData.GetTileType());
        tileObject.GetComponent<CombatGridTile>().SetTileIndex(tileData.GetTileIndex());

        if (tileData.IsWalkable())
            tileObject.GetComponent<CombatGridTile>().SetWalkable(true);
        else
        {
            var modifier = tileObject.AddComponent<NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = UnityEngine.AI.NavMesh.GetAreaFromName("Not Walkable");
        }

        tilesGO[(int)tileIndex.x + (int)tileIndex.y * _width] = tileObject;
    }

    public List<GameObject> GetAllCharacters() { return _charactersGO; }

    public List<GameObject> GetAllFriendlyCharacters()
    {
        List<GameObject> friendlyCharacters = new List<GameObject>();
        foreach(GameObject character in _charactersGO)
        {
            if(character.GetComponent<Character>().GetFaction() == Faction.Friendly)
                friendlyCharacters.Add(character);
        }
        return friendlyCharacters;
    }

    public List<GameObject> GetAllEnemyCharacters()
    {
        List<GameObject> enemyCharacters = new List<GameObject>();
        foreach (GameObject character in _charactersGO)
        {
            if (character.GetComponent<Character>().GetFaction() == Faction.Enemy)
                enemyCharacters.Add(character);
        }
        return enemyCharacters;
    }

    public void AddCharacter(CombatGridCharacterData characterData)
    {
        Vector2Int tileIndex = characterData.GetTileIndex();
        Vector3 instancePos  = characterData.GetCharacterPosition();
        Faction faction      = characterData.GetFaction();
        int healthPoints     = characterData.GetHealthPoints();
        int initiative       = characterData.GetInitiative();

        GameObject characterPrefab = _characterPrefabLibrary.GetPrefab(characterData.GetCharacterClass());
        GameObject characterObject = Object.Instantiate(characterPrefab, instancePos, Quaternion.identity);
        characterObject.GetComponent<Character>().SetCurrentTileIndex(tileIndex);
        characterObject.GetComponent<Character>().SetBaseHealthPoints(healthPoints);
        characterObject.GetComponent<Character>().SetBaseSpeed(initiative);
        characterObject.GetComponent<Character>().SetFaction(faction);

        _charactersGO.Add(characterObject);
    }
}

[System.Serializable]
public struct ClassAbilities
{
    public CharacterClass characterClass;
    public List<Ability> abilities;
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager _instance;
    
    [SerializeField] private string _fileToLoadDEBUG;

    [SerializeField] private CombatCamera _combatCamera;

    [SerializeField] private CombatState _combatState;
    [SerializeField] private CombatTurn _currentTurn;

    [SerializeField] private CombatGrid _combatGrid;
    [SerializeField] private bool _combatGridLoaded = false;

    [Header("Abilities")]
    [SerializeField] private List<ClassAbilities> _classAbilities;
    private Dictionary<CharacterClass, List<Ability>> _classAbilitiesDictionary;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        _classAbilitiesDictionary = new Dictionary<CharacterClass, List<Ability>>();
        foreach (var pair in _classAbilities)
        {
            _classAbilitiesDictionary[pair.characterClass] = pair.abilities;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _combatState = CombatState.LoadCombatLevel;
    }

    // Update is called once per frame
    void Update()
    {
        switch(_combatState)
        {
            case CombatState.LoadCombatLevel:
                {
                    HandleLoadCombatLevel();
                }break;
            case CombatState.IntroCinematic:
                {
                    HandleIntroCinematic();
                } break;
            case CombatState.PlaceCharacters:
                {
                    HandlePlaceCharacters();
                } break;
            case CombatState.MakeTurn:
                {
                    HandleMakeTurn();
                } break;
            case CombatState.EndTurn:
                {
                    HandleEndTurn();
                } break;
            case CombatState.EndCombat:
                {
                    HandleEndCombat();
                } break;
        }
    }
    
    /// <summary>
    /// Gets all abilities available to the class.
    /// </summary>
    /// <param name="characterClass">The character class to get abilities for.</param>
    /// <returns>A list of the class' available abilities.</returns>
    public List<Ability> GetClassAbilities(CharacterClass characterClass)
    {
        return _classAbilitiesDictionary.TryGetValue(characterClass, out var abilities) ? abilities : new List<Ability>();
    }

    public GameObject GetNextTurnCharacter()
    {
        int highestInitiative = Int32.MinValue;
        GameObject nextCharacter = null;
        foreach (var g in _combatGrid.GetAllCharacters())
        {
            int initiative = g.GetComponent<Character>().GetSpeed();
            if (initiative > highestInitiative)
            {
                highestInitiative = initiative;
                nextCharacter = g;
            }
        }

        return nextCharacter;
    }

    private void HandleMakeTurn()
    {
        switch(_currentTurn)
        {
            case CombatTurn.PlayerTurn:
                HandlePlayerTurn();
                break;
            case CombatTurn.EnemyTurn:
                HandleEnemyTurn();
                break;
        }
    }

    private void HandleIntroCinematic()
    {
        
        if (_combatCamera.IsIntroCinematicDone())
            _combatState = CombatState.PlaceCharacters;
        else
            _combatCamera.PlayIntroCinematic();
    }

    private void HandleLoadCombatLevel()
    {
        if(!_combatGridLoaded)
        {
            _combatGridLoaded = true;
            LoadNextLevel();
            GameObject NavMesh = GameObject.Find("NavMesh Surface");
            NavMesh.GetComponent<NavMeshSurface>().BuildNavMesh();
            _combatState = CombatState.IntroCinematic;
        }
    }

    private void HandlePlaceCharacters()
    {

    }
    private void HandleEndTurn()
    {

    }

    private void HandlePlayerTurn()
    {
        GameObject nextCharacter = GetNextTurnCharacter();
        
        // TODO: Call selector with character.

    }

    private void HandleEnemyTurn()
    {

    }

    private void HandleEndCombat()
    {

    }

    private void LoadNextLevel()
    {
        
        string filePathToload = Application.dataPath + "\\JSON BattleGrids\\" + _fileToLoadDEBUG + ".json";

        if (!System.IO.File.Exists(filePathToload))
        {
            Debug.Log("Level File didn't exist or filepath was wrong!");
            return;
        }

        string jsonFileData = System.IO.File.ReadAllText(filePathToload);
        if(jsonFileData.Length == 0)
        {
            Debug.Log("json File Data was empty!");
            return;
        }

        CombatGridSerializedSaveData combatGrid = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        this._combatGrid.SetCombatGridSize(combatGrid._gridWidth, combatGrid._gridHeight);
        this._combatGrid.SetTileSize(combatGrid._tileSize);
        Debug.Log("CombatGrid tileSize: " + combatGrid._tileSize);
       
        for (int i = 0; i < combatGrid._tileData.Count; i++)
        {
            Debug.Log("tiled["+i+"]: " + "\tTileType : " + combatGrid._tileData[i].GetTileType() + 
                      "\tTileIndex: " + combatGrid._tileData[i].GetTilePosition() + "\n");

            this._combatGrid.AddTile(combatGrid._tileData[i]);
        }
        
        for(int i = 0; i < combatGrid._characterData.Count; i++)
        {
            this._combatGrid.AddCharacter(combatGrid._characterData[i]);
        }
        
    }

    private void EvaluateInitiativeOrder()
    {

    }

    public List<GameObject> GetAllCharacters()
    {
        return _combatGrid.GetAllCharacters();

    }

    public List<GameObject> GetEnemyCharacters()
    {
        return _combatGrid.GetAllEnemyCharacters();
    }

    public List<GameObject> GetAllFriendlyCharacters()
    {
        return _combatGrid.GetAllFriendlyCharacters();
    }
    public Vector3 GetTileSize()
    {
        return _combatGrid.GetTileSize();
    }

    public int GetGridWidth()
    {
        return _combatGrid.GetGridWidth();
    }

    public int GetGridHeight()
    {
        return _combatGrid.GetGridHeight();
    }

    public GameObject[] GetGridTiles()
    {
        return _combatGrid.GetAllTiles();
    }

    public GameObject GetTileAtCoord(int x, int y)
    {
        return _combatGrid.GetTileAtCoord(x, y);
    }
    public CombatGridTile GetTileComponent(int x, int y)
    {
        GameObject tileObject = GetTileAtCoord(x, y);
        if (tileObject == null) return null;

        return tileObject.GetComponent<CombatGridTile>();
    }

    public CombatTurn GetCombatTurn()
    {
        return _currentTurn;
    }

    public void SetCombatTurn(CombatTurn turn) 
    { 
        _currentTurn = turn; 
    }
}
