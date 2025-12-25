using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class CombatGrid : MonoBehaviour
{
    public static CombatGrid _instance;

    [SerializeField] private TilePrefabLibrary _tilePrefabLibrary;
    [SerializeField] private CharacterPrefabLibrary _characterPrefabLibrary;

    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private Vector3 _tileSize;

    [SerializeField] private bool _bCombatGridLoaded;

    [SerializeField] private GameObject[] _tilesGO;
    [SerializeField] private GameObject[] _offgridSpawnTilesGO;
    [SerializeField] private List<GameObject> _charactersGO;
    
    [SerializeField] private Material inCombatTileMaterial;
    [SerializeField] private Material inCombatTileMaterialLava;
    [SerializeField] private Material inCombatTileMaterialPoison;

    [SerializeField] private string _fileToLoadDEBUG;

    private GameObject _friendlyCharacterRoot;
    private GameObject _enemyCharacterRoot;
    private GameObject _tileRoot;

    private void Awake()
    {
        if (_instance == null)
        {
            DebugLog.CJLog("CombatGrid Awake(), instance = " + CombatGrid._instance);
            _instance = this;
            DebugLog.CJLog("CombatGrid instance now = " + CombatGrid._instance);

            // NOTE (Calle): Can't be a Dont' destroy on load if its a child to the Combat Manager, (So maybe make it root for itself?)
            //DontDestroyOnLoad(gameObject);


            _tilePrefabLibrary      = Resources.Load<TilePrefabLibrary>("Tiles/TilePrefabLibrary");
            _characterPrefabLibrary = Resources.Load<CharacterPrefabLibrary>("Characters/CharacterPrefabLibrary");

            if (_tilePrefabLibrary == null)
                DebugLog.CJLog("CombatGrid failed to load TilePrefabLibrary.");
            if (_tilePrefabLibrary == null)
                DebugLog.CJLog("CombatGrid failed to load CharacterPrefabLibrary.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        CombatEventManager.OnCharacterDeath += HandleCharacterDeath;
        CombatEventManager.OnExitCombatStatePlaceCharacter += OnExitPlaceCharacter;

    }

    private void OnDisable()
    {
        CombatEventManager.OnCharacterDeath -= HandleCharacterDeath;
        CombatEventManager.OnExitCombatStatePlaceCharacter -= OnExitPlaceCharacter;
    }

    public void Start()
    {
        _friendlyCharacterRoot = new GameObject();
        _friendlyCharacterRoot.name = "-PLAYER PARTY-";

        _enemyCharacterRoot = new GameObject();
        _enemyCharacterRoot.name = "-ENEMY CHARACTERS-";

        _tileRoot = new GameObject();
        _tileRoot.name = "-GRID TILES-";

        _offgridSpawnTilesGO = new GameObject[4];
    }

    public bool IsCombatGridLoaded() { return _bCombatGridLoaded; }
    public GameObject[] GetAllTiles() { return _tilesGO; }
    public List<Character> GetAllCharacterScripts() 
    {
        List<Character> characterScritps = new List<Character>();

        foreach(GameObject characterGO in GetAllCharacters())
        {
            Character character = characterGO.GetComponent<Character>();
            if(character)
            {
                characterScritps.Add(character);
            }
        }
        return characterScritps; 
    }

    public List<Character> GetCharacterScriptsByFaction(Faction faction)
    {
        List<Character> characterScritps = new List<Character>();

        foreach (GameObject characterGO in GetAllCharacters())
        {
            Character character = characterGO.GetComponent<Character>();
            if (character)
            {
                if (character.GetFaction() == faction)
                {
                    characterScritps.Add(character);
                }
            }
        }
        return characterScritps;
    }

    public List<CombatGridTile> GetAllCombatGridTileScripts() 
    {
        List<CombatGridTile> tiles = new List<CombatGridTile>();

        foreach(GameObject tileGO in _tilesGO)
        {
            CombatGridTile combatGridTile = tileGO.GetComponent<CombatGridTile>();
            if (combatGridTile)
                tiles.Add(combatGridTile);
        }
        return tiles; 
    }

    public List<CombatGridTile> GetAllDeployTiles()
    {
        List<CombatGridTile> deployTiles = new List<CombatGridTile>();

        foreach(GameObject tileGO in _tilesGO)
        {
            CombatGridTile combatGridTile = tileGO.GetComponent<CombatGridTile>();
            if(combatGridTile.GetTileType() == TileType.Deploy)
            {
                deployTiles.Add(combatGridTile);
            }
        }
        return deployTiles;
    }
    public GameObject GetTileAtCoord(int x, int y)
    {
        int index = x + y * _width;
        // if (index < 0 || index >= _width * _height)
        //     return null;
        
        if (_tilesGO == null)
        {
            DebugLog.CJLog("GetTileAtCoord FAILED: _tilesGO is NULL!");
            return null;
        }

        if (_tilesGO.Length == 0)
        {
            DebugLog.CJLog("GetTileAtCoord FAILED: _tilesGO is EMPTY!");
            return null;
        }

        if (index < 0 || index >= _tilesGO.Length)
        {
            DebugLog.CJLog($"GetTileAtCoord FAILED: index {index} OUT OF RANGE (length={_tilesGO.Length})");
            return null;
        }

        if (_tilesGO[index] == null)
        {
            DebugLog.CJLog($"GetTileAtCoord FAILED: tile at index {index} is NULL!");
            return null;
        }

        return _tilesGO[index];
    }

    public CombatGridTile[] GetOffGridTiles()
    {
        CombatGridTile[] tileScripts = new CombatGridTile[4];
        for(int i = 0; i < tileScripts.Length; i++)
        {
            tileScripts[i] = _offgridSpawnTilesGO[i].GetComponent<CombatGridTile>();
        }
        
        return tileScripts;
    }

    public void HideOffGridTiles()
    {
        for(int i = 0; i < _offgridSpawnTilesGO.Length; i++)
        {
            _offgridSpawnTilesGO[i].SetActive(false);
        }
    }

    public void ShowOffGridTiles()
    {
        for (int i = 0; i < _offgridSpawnTilesGO.Length; i++)
        {
            _offgridSpawnTilesGO[i].SetActive(true);
        }
    }

    public bool AllCharactersPlaced()
    {
        for(int i = 0; i < _offgridSpawnTilesGO.Length; i++)
        {
            CombatGridTile tileScript = _offgridSpawnTilesGO[i].GetComponent<CombatGridTile>();
            if (tileScript.GetOccupant() != null)
                return false;
        }
        
        return true;
    }

    public Vector3 GetTileSize() { return _tileSize; }
    public int GetGridWidth() { return _width; }
    public int GetGridHeight() { return _height; }
    public void SetCombatGridSize(int w, int h)
    {
        _width = w;
        _height = h;
        _tilesGO = new GameObject[w * h];
    }
    public void SetTileSize(Vector3 tileSize)
    {
        _tileSize = tileSize;
    }

    public bool ContainsCharacter(GameObject chracter) { return _charactersGO.Contains(chracter); }
    public GameObject AddOffgridTile(CombatGridTileData tileData)
    {
        GameObject result = null;
        if (tileData.GetTileType() == TileType.UnInitialized)
            return null;

        Vector2 tileIndex = tileData.GetTileIndex();
        Vector3 instancePos = tileData.GetTilePosition();

        if (_tilePrefabLibrary != null)
        {
            GameObject tilePrefab = _tilePrefabLibrary.GetPrefab(tileData.GetTileType());
            GameObject tileObject = Object.Instantiate(tilePrefab, instancePos, Quaternion.identity);
            result = tileObject;

            tileObject.transform.localScale = tileData.GetTileSize();
            tileObject.GetComponent<CombatGridTile>().SetTilePosition(tileData.GetTilePosition());
            tileObject.GetComponent<CombatGridTile>().SetTileType(tileData.GetTileType());
            tileObject.GetComponent<CombatGridTile>().SetTileIndex(tileData.GetTileIndex());

            MeshRenderer meshRend = tileObject.GetComponent<MeshRenderer>();
            Material inCombatTileMaterial = Resources.Load<Material>("Shaders/Tiles/TileMaterial");

            if (inCombatTileMaterial != null)
            {
                meshRend.material = inCombatTileMaterial;
                meshRend.material.SetColor("_TileColor", Color.white);
            }
            else
            {
                DebugLog.CJLog("Failed to load TileMaterial.mat");

            }

            _offgridSpawnTilesGO[(int)tileIndex.x + (int)tileIndex.y] = tileObject;
        }
        else
        {
            DebugLog.CJLog("No TilePrefabLibrary assigned in inspector!");
        }

        return result;
    }
    public GameObject AddTile(CombatGridTileData tileData)
    {
        GameObject result = null;
        if (tileData.GetTileType() == TileType.UnInitialized)
            return null;

        Vector2 tileIndex = tileData.GetTileIndex();
        Vector3 instancePos = tileData.GetTilePosition();

        if (_tilePrefabLibrary != null)
        {
            GameObject tilePrefab = _tilePrefabLibrary.GetPrefab(tileData.GetTileType());
            GameObject tileObject = Object.Instantiate(tilePrefab, instancePos, Quaternion.identity);
            result = tileObject;

            tileObject.transform.localScale = tileData.GetTileSize();
            Vector3 size = tileObject.transform.localScale;
            size.y = 0.0001f;
            tileObject.transform.localScale = size;
            tileObject.GetComponent<CombatGridTile>().SetTilePosition(tileData.GetTilePosition());
            tileObject.GetComponent<CombatGridTile>().SetTileType(tileData.GetTileType());
            tileObject.GetComponent<CombatGridTile>().SetTileIndex(tileData.GetTileIndex());

            MeshRenderer meshRend = tileObject.GetComponent<MeshRenderer>();
            Material inCombatTileMaterial = Resources.Load<Material>("Shaders/Tiles/TileMaterial");
            

            switch (tileData.GetTileType())
            {
                case TileType.UnInitialized:
                    {

                    }
                    break;
                case TileType.Impassable:
                    {
                        if (meshRend != null)
                        {
                            meshRend.material = inCombatTileMaterial;
                            meshRend.material.SetFloat("_Alpha", 0.0f);
                            meshRend.material.SetColor("_TileColor", Color.black);
                        }
                    }
                    break;
                case TileType.Deploy:
                    {
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterial;
                            meshRend.material.SetColor("_TileColor", Color.green);
                        }
                        else
                        {
                            DebugLog.CJLog("Failed to load TileMaterial.mat");

                        }
                    }
                    break;
                case TileType.Lava:
                    {
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterialLava;
                            //meshRend.material.SetVector("_TextureTileCoord", new Vector2(0, 1));
                        }
                    } break;
                case TileType.Poison:
                    {
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterialPoison;
                            //meshRend.material.SetVector("_TextureTileCoord", new Vector2(2, 0));
                        }
                    }
                    break;
                default:
                    {
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterial;
                            meshRend.material.SetColor("_TileColor", Color.white);
                        }
                        else
                        {
                            DebugLog.CJLog("Failed to load TileMaterial.mat");

                        }
                    }
                    break;
            }

            _tilesGO[(int)tileIndex.x + (int)tileIndex.y * _width] = tileObject;
        }
        else
        {
            DebugLog.CJLog("No TilePrefabLibrary assigned in inspector!");
        }

        return result;
    }

    public List<GameObject> GetAllCharacters() { return _charactersGO; }

    public List<GameObject> GetAllFriendlyCharacters()
    {
        List<GameObject> friendlyCharacters = new List<GameObject>();
        foreach (GameObject character in _charactersGO)
        {
            Character characterScript = character.GetComponent<Character>();
            if (characterScript != null)
            {
                if (characterScript.GetFaction() == Faction.Friendly)
                    friendlyCharacters.Add(character);
            }
        }
        return friendlyCharacters;
    }

    public void ClearOffGridOccupant(GameObject character)
    {
        foreach (var offTileGO in _offgridSpawnTilesGO)
        {
            if (!offTileGO)
                continue;
            
            var tile = offTileGO.GetComponent<CombatGridTile>();
            if (tile && tile.GetOccupant() == character)
            {
                tile.SetOccupant(null);
            }
        }
    }

    public List<GameObject> GetAllEnemyCharacters()
    {
        List<GameObject> enemyCharacters = new List<GameObject>();
        foreach (GameObject character in _charactersGO)
        {
            if (character == null)
                continue;

            Character characterScript = character.GetComponent<Character>();
            if (characterScript != null) 
            {
                if (characterScript.GetFaction() == Faction.Enemy)
                    enemyCharacters.Add(character);
            }
            
        }
        return enemyCharacters;
    }

    public GameObject AddCharacter(CombatGridCharacterData characterData)
    {
        Vector3 instancePos = characterData.GetCharacterPosition();
        Quaternion rotation = characterData.GetRotation();
        Vector2Int tileIndex = characterData.GetCurrentTileIndex();
        Faction faction = characterData.GetFaction();
        CharacterClass characterClass = characterData.GetCharacterClass();

        // Instantiate prefab
        GameObject characterPrefab = _characterPrefabLibrary.GetPrefab(characterClass);
        GameObject characterObject = Object.Instantiate(characterPrefab, instancePos, rotation);
        Character characterScript = characterObject.GetComponent<Character>();

        // Initialize CharacterData directly from JSON
        characterScript.Data.InitializeFromJSON(
            faction,
            characterData.GetBaseHealthPoints(),
            characterData.GetBaseDamage(),
            characterData.GetBaseInitiative(),
            characterData.GetBaseMovementPoints(),
            characterData.GetHealthPoints()
        );

        // Set class, faction, tile
        characterScript.SetCharacterClass(characterClass);
        characterScript.SetFaction(faction);
        characterScript.SetCurrentTileIndex(tileIndex);

        // Add UI/frames
        characterScript.AddCharacterFrame();

        // Optional: any additional Character setup
        characterScript.Initialize(characterScript.Data);

        // Add to grid
        _charactersGO.Add(characterObject);

        return characterObject;
    }

    private void HandleCharacterDeath(Character character)
    {
        RemoveCharacter(character.gameObject);
    }

    public void RemoveCharacter(GameObject character)
    {
        _charactersGO.Remove(character);
    }
    public void SpawnCharacter(CombatGridCharacterData combatGridCharacterData)
    {
        GameObject character = AddCharacter(combatGridCharacterData);
        if (combatGridCharacterData.GetFaction() == Faction.Friendly)
            character.transform.SetParent(_friendlyCharacterRoot.transform);
        else if(combatGridCharacterData.GetFaction() == Faction.Enemy)
            character.transform.SetParent(_enemyCharacterRoot.transform);
    }

    public Character SpawnCharacter(CharacterData data, Vector3 position, Quaternion rotation)
    {
        // Instantiate the prefab
        var characterGO = GameObject.Instantiate(_characterPrefabLibrary.GetPrefab(data.CharacterClass), position, rotation);

        // Get the Character component
        Character character = characterGO.GetComponent<Character>();
        Faction faction = character.GetFaction();

        if (faction == Faction.Friendly)
        {
            characterGO.layer = LayerMask.NameToLayer("Friendly");
        }
        else if (faction == Faction.Enemy)
        {
            characterGO.layer = LayerMask.NameToLayer("Enemy");
        }
        // Assign and initialize
        character.Initialize(data);

        character.AddCharacterFrame();

        _charactersGO.Add(characterGO);

        if (character.GetFaction() == Faction.Friendly)
            characterGO.transform.SetParent(_friendlyCharacterRoot.transform);
        else if (character.GetFaction() == Faction.Enemy)
            characterGO.transform.SetParent(_enemyCharacterRoot.transform);

        return character;
    }

    public void LoadNextLevel()
    {
        string filePathToLoad = Application.streamingAssetsPath + "/JSON/CombatGrids/" + _fileToLoadDEBUG + ".json";

        if (!System.IO.File.Exists(filePathToLoad))
        {
            DebugLog.CJLog("Level File didn't exist or filepath was wrong!");
            return;
        }

        string jsonFileData = System.IO.File.ReadAllText(filePathToLoad);
        if (jsonFileData.Length == 0)
        {
            DebugLog.CJLog("json File Data was empty!");
            return;
        }

        CombatGridSerializedSaveData combatGridSaveData = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        SetCombatGridSize(combatGridSaveData._gridWidth, combatGridSaveData._gridHeight);
        SetTileSize(combatGridSaveData._tileSize);
        DebugLog.CJLog("CombatGrid tileSize: " + combatGridSaveData._tileSize);



        for (int i = 0; i < combatGridSaveData._tileData.Count; i++)
        {
            //DebugLog.CJLog("tiled["+i+"]: " + "\tTileType : " + combatGridSaveData._tileData[i].GetTileType() + 
            //          "\tTileIndex: " + combatGridSaveData._tileData[i].GetTilePosition() + "\n");

            AddTile(combatGridSaveData._tileData[i]).transform.SetParent(_tileRoot.transform);

        }

        float sizeOffGrid = 4f;
        float startZ = (combatGridSaveData._gridHeight - sizeOffGrid) + 1f;
        for (int i = 0; i < 4; i++)
        {
            CombatGridTileData tileData = new CombatGridTileData(TileType.Walkable,
                                                                 new Vector2Int(0, i),
                                                                 new Vector3(-2f, 0, startZ + (2 * i)),
                                                                 new Vector3(2f, 0.0001f, 2f));
            AddOffgridTile(tileData);
        }

        for (int i = 0; i < combatGridSaveData._characterData.Count; i++)
        {
            AddCharacter(combatGridSaveData._characterData[i]).transform.SetParent(_enemyCharacterRoot.transform);
        }

        _bCombatGridLoaded = true;
    }

    private void OnExitPlaceCharacter()
    {
        foreach(GameObject tile in GetAllTiles())
        {
            MeshRenderer meshRend = tile.GetComponent<MeshRenderer>();

            switch(tile.GetComponent<CombatGridTile>().GetTileType())
            {
                case TileType.UnInitialized:
                    {

                    }
                    break;
                case TileType.Impassable:
                    {
                        if (meshRend != null)
                        {
                            meshRend.material.SetFloat("_Alpha", 0.0f);
                        }
                    }
                    break;
                case TileType.Lava:
                    {
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterialLava;
                            //meshRend.material.SetVector("_TextureTileCoord", new Vector2(0, 1));
                            //meshRend.material.SetColor("_TileColor", Color.white);
                        }
                    }
                    break;
                case TileType.Poison:
                    {
                        if (inCombatTileMaterial != null)
                        {
                            meshRend.material = inCombatTileMaterialPoison;
                            //meshRend.material.SetVector("_TextureTileCoord", new Vector2(2, 0));
                            //meshRend.material.SetColor("_TileColor", Color.white);
                        }
                    }
                    break;
                default:
                    {
                        if (meshRend != null)
                        {
                            meshRend.material.SetColor("_TileColor", Color.white);
                        }
                    }
                    break;
            }
            
        }
    }
}
