using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
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
    
    [SerializeField] private GameObject[] _tilesGO;
    [SerializeField] private List<GameObject> _charactersGO;


    public GameObject[] GetAllTiles() {  return _tilesGO; }
    public GameObject GetTileAtCoord(int x, int y) 
    {
        int index = x + y * _width;
        if (index < 0 || index >= _width * _height)
            return null;

        return _tilesGO[index];  
    }

    public Vector3 GetTileSize() { return _tileSize; }
    public int GetGridWidth() { return _width; }
    public int GetGridHeight() { return _height; }
    public void SetCombatGridSize(int w, int h)
    {
        _width  = w;
        _height = h;
        _tilesGO = new GameObject[w * h];
    }
    public void SetTileSize(Vector3 tileSize)
    {
        _tileSize = tileSize;
    }

    public bool ContainsCharacter(GameObject chracter) { return _charactersGO.Contains(chracter); }

    public void AddTile(CombatGridTileData tileData)
    {
        if (tileData.GetTileType() == TileType.UnInitialized)
            return;

        Vector2 tileIndex = tileData.GetTileIndex();
        Vector3 instancePos = tileData.GetTilePosition();

        if(_tilePrefabLibrary != null)
        {
            GameObject tilePrefab = _tilePrefabLibrary.GetPrefab(tileData.GetTileType());
            GameObject tileObject = Object.Instantiate(tilePrefab, instancePos, Quaternion.identity);

            tileObject.transform.localScale = tileData.GetTileSize();
            tileObject.GetComponent<CombatGridTile>().SetTilePosition(tileData.GetTilePosition());
            tileObject.GetComponent<CombatGridTile>().SetTileType(tileData.GetTileType());
            tileObject.GetComponent<CombatGridTile>().SetTileIndex(tileData.GetTileIndex());
            
            switch(tileData.GetTileType())
            {
                case TileType.Deploy:
                    {

                    }break;
                default:
                    {
                        MeshRenderer meshRend = tileObject.GetComponent<MeshRenderer>();
                        Material inCombatTileMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/CJ Test Shaders/TileMaterial.mat");
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterial;
                            if (tileObject.GetComponent<CombatGridTile>().GetTileIndex().x == 0)
                                meshRend.material.SetColor("_TileColor", Color.green);
                        }
                        else
                        {
                            DebugLog.CJLog("Failed to load TileMaterial.mat");

                        }
                    } break;}
           
            if (tileData.IsWalkable())
                tileObject.GetComponent<CombatGridTile>().SetWalkable(true);
            else
            {
                var volume = tileObject.AddComponent<NavMeshModifierVolume>();
                volume.area = NavMesh.GetAreaFromName("Not Walkable");
            
                Vector3 tileSize = tileData.GetTileSize();
                volume.size = new Vector3(1.0f, 2.0f, 1.0f);
                volume.center = new Vector3(0, 0.5f, 0);
            }

            _tilesGO[(int)tileIndex.x + (int)tileIndex.y * _width] = tileObject;
        }
        else
        {
            DebugLog.CJLog("No TilePrefabLibrary assigned in inspector!");
        }
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
        Vector2Int tileIndex    = characterData.GetTileIndex();
        Vector3    instancePos  = characterData.GetCharacterPosition();
        Quaternion rotation     = characterData.GetRotation();
        Faction    faction      = characterData.GetFaction();
        int        healthPoints = characterData.GetHealthPoints();
        int        initiative   = characterData.GetInitiative();

        GameObject characterPrefab = _characterPrefabLibrary.GetPrefab(characterData.GetCharacterClass());
        GameObject characterObject = Object.Instantiate(characterPrefab, instancePos, rotation);

        characterObject.GetComponent<Character>().SetCurrentTileIndex(tileIndex);
        characterObject.GetComponent<Character>().SetBaseHealthPoints(healthPoints);
        characterObject.GetComponent<Character>().SetBaseSpeed(initiative);
        characterObject.GetComponent<Character>().SetFaction(faction);

        _charactersGO.Add(characterObject);
    }

    public void RemoveCharacter(GameObject character)
    {
        _charactersGO.Remove(character);
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
    private Selector _selector;

    [SerializeField] private string _fileToLoadDEBUG;

    [SerializeField] private CombatCamera _combatCamera;
    [SerializeField] private float _cameraSpeed;

    [SerializeField] private CombatState _combatState;
    [SerializeField] private CombatTurn _currentTurn;

    [SerializeField] private CombatGrid _combatGrid;
    [SerializeField] private bool _combatGridLoaded = false;

    [Header("Abilities")]
    [SerializeField] private List<ClassAbilities> _classAbilities;
    private Dictionary<CharacterClass, List<Ability>> _classAbilitiesDictionary;

    public UnityEvent EnemyTurnStart = new();
    
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
        _selector = GetComponent<Selector>();
    }

    
    // Update is called once per frame
    void Update()
    {
        MoveCamera();

        switch (_combatState)
        {
            case CombatState.LoadCombatLevel:
                {
                    HandleLoadCombatLevel();

                    Vector2Int tileIndex = new Vector2Int(5, 0);
                    Vector3 position = new Vector3(1.0f + tileIndex.x * 2.0f, 0.0f, 1.0f + tileIndex.y * 2.0f);
                    CombatGridCharacterData characterData = new CombatGridCharacterData(CharacterClass.Wizard,
                                                                                       Faction.Enemy,
                                                                                       10,
                                                                                       1,
                                                                                       tileIndex,
                                                                                       position,
                                                                                       Vector3.one,
                                                                                       Quaternion.identity);
                    _combatGrid.AddCharacter(characterData);
                }
                break;
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

    private void MoveCamera()
    {
        Vector3 cameraMovement = Vector3.zero;
        Vector3 cameraSpeedVector = new Vector3(_cameraSpeed, _cameraSpeed, _cameraSpeed);
        
        if (Input.GetKey(KeyCode.D))
            cameraMovement += Vector3.right;
        if (Input.GetKey(KeyCode.A))
            cameraMovement += Vector3.left;
        if (Input.GetKey(KeyCode.W))
            cameraMovement += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            cameraMovement += Vector3.back;

        cameraMovement = Vector3.Scale(cameraMovement, cameraSpeedVector);
        
        cameraMovement *= Time.deltaTime;

        if(cameraMovement != Vector3.zero)
            _combatCamera.transform.position = cameraMovement + _combatCamera.transform.position;
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
           
            // TODO (Calle): Detta ska göra i LevelManagern
            LoadNextLevel();
            //LoadCurrentPlayerParty();
            GameObject NavMesh = GameObject.Find("NavMesh Surface");
            NavMesh.GetComponent<NavMeshSurface>().BuildNavMesh();
            _combatState = CombatState.IntroCinematic;
        }
    }

    private void HandlePlaceCharacters()
    {
        if(_selector)
        {
            _selector.SetCurrentState(SelectorState.PlacingCharacters);

            _selector.UpdatePlaceCharacter(_combatGrid.GetAllTiles());
            
            CombatGridTile unoccupiedDeployTile = _selector.GetUnoccupiedDeployTileClicked();
            Character selectedCharacter = _selector.GetSelectedCharacter();
            
            if(selectedCharacter)
            {
                if (unoccupiedDeployTile)
                {
                    if(_combatGrid.ContainsCharacter(selectedCharacter.gameObject))
                    {

                    }
                    else
                    {
                        Vector2Int tileIndex = unoccupiedDeployTile.GetTileIndex();
                        Vector3 tilePosition = unoccupiedDeployTile.GetTilePosition();
                        Vector3 slitghtlyRaisedPosition = new Vector3(tilePosition.x, tilePosition.y + 0.05f, tilePosition.z);

                        // TODO (Calle): Get the actuall characterData from GameStateManager
                        //               For now spawn a stub character.

                        CombatGridCharacterData characterData = new CombatGridCharacterData(CharacterClass.Wizard,
                                                                                            Faction.Friendly,
                                                                                            10,
                                                                                            1,
                                                                                            tileIndex,
                                                                                            tilePosition,
                                                                                            Vector3.one,
                                                                                            Quaternion.identity);
                        _combatGrid.AddCharacter(characterData);
                    }
                }
                else
                {
                    DebugLog.CJLog("Show ERROR UI to place on a deploy tile.");
                }
            }
            
        }

        //_selector.ResetSelectedCharacter();
    }

    private void HandleEndTurn()
    {

    }

    private void HandlePlayerTurn()
    {
        // TODO (Calle): 
        //  Vid starten av varje hero karaktärs turn sker dessa saker: 
        //  - Spelarens mana ökar med 1 -> I CardHandManager()
        //  - Hero karaktärens ability cooldowns minskar med 1 -> WIP (MG/JOPPA)
        //  - Spelarens "cooldown" / timer för att dra ett till kort minskar med 1 -> WIP 
        
        GameObject nextCharacter = GetNextTurnCharacter();

        // TODO: Call selector with character.
        Selector._instance.SetCurrentState(SelectorState.Idle);
    }

    private void HandleEnemyTurn()
    {
        EnemyTurnStart.Invoke();
    }

    private void HandleEndCombat()
    {

    }

    private void LoadNextLevel()
    {
        CombatGrid combatGrid1 = CombatManager._instance.GetCombatGrid();

        string filePathToload = Application.dataPath + "\\JSON BattleGrids\\" + _fileToLoadDEBUG + ".json";

        if (!System.IO.File.Exists(filePathToload))
        {
            DebugLog.CJLog("Level File didn't exist or filepath was wrong!");
            return;
        }

        string jsonFileData = System.IO.File.ReadAllText(filePathToload);
        if(jsonFileData.Length == 0)
        {
            DebugLog.CJLog("json File Data was empty!");
            return;
        }

        CombatGridSerializedSaveData combatGrid = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        this._combatGrid.SetCombatGridSize(combatGrid._gridWidth, combatGrid._gridHeight);
        this._combatGrid.SetTileSize(combatGrid._tileSize);
        DebugLog.CJLog("CombatGrid tileSize: " + combatGrid._tileSize);
       
        for (int i = 0; i < combatGrid._tileData.Count; i++)
        {
            DebugLog.CJLog("tiled["+i+"]: " + "\tTileType : " + combatGrid._tileData[i].GetTileType() + 
                      "\tTileIndex: " + combatGrid._tileData[i].GetTilePosition() + "\n");

            this._combatGrid.AddTile(combatGrid._tileData[i]);
        }
        
        for(int i = 0; i < combatGrid._characterData.Count; i++)
        {
            this._combatGrid.AddCharacter(combatGrid._characterData[i]);
        }
        
    }

    private void LoadCurrentPlayerParty()
    {
        
    }

    private void EvaluateInitiativeOrder()
    {

    }

    public CombatGrid GetCombatGrid()
    {
        return this._combatGrid;
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
