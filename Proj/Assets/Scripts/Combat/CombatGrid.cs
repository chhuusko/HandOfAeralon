using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CombatGrid : MonoBehaviour
{
    public static CombatGrid _instance;

    [SerializeField] private TilePrefabLibrary _tilePrefabLibrary;
    [SerializeField] private CharacterPrefabLibrary _characterPrefabLibrary;

    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private Vector3 _tileSize;

    [SerializeField] private GameObject[] _tilesGO;
    [SerializeField] private List<GameObject> _charactersGO;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _tilePrefabLibrary      = AssetDatabase.LoadAssetAtPath<TilePrefabLibrary>("Assets/ScriptableObject/Tiles/TilePrefabLibrary.asset");
            _characterPrefabLibrary = AssetDatabase.LoadAssetAtPath<CharacterPrefabLibrary>("Assets/ScriptableObject/Characters/CharacterPrefabLibrary.asset");
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

    public GameObject[] GetAllTiles() { return _tilesGO; }
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
        _width = w;
        _height = h;
        _tilesGO = new GameObject[w * h];
    }
    public void SetTileSize(Vector3 tileSize)
    {
        _tileSize = tileSize;
    }

    public bool ContainsCharacter(GameObject chracter) { return _charactersGO.Contains(chracter); }

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
            tileObject.GetComponent<CombatGridTile>().SetTilePosition(tileData.GetTilePosition());
            tileObject.GetComponent<CombatGridTile>().SetTileType(tileData.GetTileType());
            tileObject.GetComponent<CombatGridTile>().SetTileIndex(tileData.GetTileIndex());

            switch (tileData.GetTileType())
            {
                case TileType.Deploy:
                    {

                    }
                    break;
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
                    }
                    break;
            }

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

        return result;
    }

    public List<GameObject> GetAllCharacters() { return _charactersGO; }

    public List<GameObject> GetAllFriendlyCharacters()
    {
        List<GameObject> friendlyCharacters = new List<GameObject>();
        foreach (GameObject character in _charactersGO)
        {
            if (character.GetComponent<Character>().GetFaction() == Faction.Friendly)
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

    public GameObject AddCharacter(CombatGridCharacterData characterData)
    {
        GameObject result = null;

        Vector2Int tileIndex = characterData.GetTileIndex();
        Vector3 instancePos = characterData.GetCharacterPosition();
        Quaternion rotation = characterData.GetRotation();
        Faction faction = characterData.GetFaction();
        int healthPoints = characterData.GetHealthPoints();
        int initiative = characterData.GetInitiative();

        GameObject characterPrefab = _characterPrefabLibrary.GetPrefab(characterData.GetCharacterClass());
        GameObject characterObject = Object.Instantiate(characterPrefab, instancePos, rotation);
        result = characterObject;

        characterObject.GetComponent<Character>().SetCurrentTileIndex(tileIndex);
        characterObject.GetComponent<Character>().SetBaseHealthPoints(healthPoints);
        characterObject.GetComponent<Character>().SetBaseSpeed(initiative);
        characterObject.GetComponent<Character>().SetFaction(faction);

        _charactersGO.Add(characterObject);

        return result;
    }

    public void RemoveCharacter(GameObject character)
    {
        _charactersGO.Remove(character);
    }
}
