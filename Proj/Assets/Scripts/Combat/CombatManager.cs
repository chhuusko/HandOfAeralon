using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private TilePrefabLibrary tilePrefabLibrary;

    CombatGridTile[,] tiles;
    [SerializeField] private GameObject[] tilesGO;
    [SerializeField] private int _height;
    [SerializeField] private int _width;
    public GameObject[] GetAllTiles() {  return tilesGO; }
    public GameObject GetTileAtCoord(int x, int y) { return tilesGO[x + y * _width];  }
    public int GetGridWidth() { return _width; }
    public int GetGridHeight() { return _height; }
    public void SetCombatGridSize(int w, int h)
    {
        _width  = w;
        _height = h;
        tilesGO = new GameObject[w * h];
        tiles   = new CombatGridTile[w, h];
    } 

    public void AddTile(CombatGridTileData tileData)
    {
        //CombatGridTile tile = new CombatGridTile(tileData);
        Vector2 position = tileData.GetTilePosition();
        //tiles[(int)position.x, (int)position.y] = tile;

        Vector3 instancePos = new Vector3(position.x, 0.0f, position.y);
        Debug.Log("Is Walkable: " + tileData.IsWalkable());
        GameObject tileObject = Object.Instantiate(tilePrefabLibrary.GetPrefab(tileData.GetTileType()), instancePos, Quaternion.identity);

        if(tileData.IsWalkable())
            tileObject.GetComponent<CombatGridTile>().SetWalkable(true);

        tilesGO[(int)position.x + (int)position.y * _width] = tileObject;
    }
}

public class CombatManager : MonoBehaviour
{
    [SerializeField] private CombatCamera _combatCamera;

    [SerializeField] private CombatState combatState;
    [SerializeField] private CombatTurn currentTurn;

    [SerializeField] private CombatGrid combatGrid;
    [SerializeField] private bool combatGridLoaded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatState = CombatState.LoadCombatLevel;
    }

    // Update is called once per frame
    void Update()
    {
        switch(combatState)
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


    private void HandleMakeTurn()
    {
        switch(currentTurn)
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
            combatState = CombatState.PlaceCharacters;
        else
            _combatCamera.PlayIntroCinematic();
    }

    private void HandleLoadCombatLevel()
    {
        if(!combatGridLoaded)
        {
            combatGridLoaded = true;
            LoadNextLevel();
            combatState = CombatState.IntroCinematic;
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

    }

    private void HandleEnemyTurn()
    {

    }

    private void HandleEndCombat()
    {

    }

    private void LoadNextLevel()
    {
        string fileName = "BattleGridWithSize";
        string filePathToload = Application.dataPath + "\\JSON BattleGrids\\" + fileName + ".json";

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

        CombatGridSerializedSaveData tileGrid = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        combatGrid.SetCombatGridSize(tileGrid.gridWidth, tileGrid.gridHeight);

        for (int i = 0; i < tileGrid.tileData.Count; i++)
        {
            Debug.Log("TileType : " + tileGrid.tileData[i].GetTileType() + 
                      "\nTilePosition: " + tileGrid.tileData[i].GetTilePosition());

            combatGrid.AddTile(tileGrid.tileData[i]);
        }

    }

    private void EvaluateInitiativeOrder()
    {

    }

    public GameObject[] GetGridTiles()
    {
        return combatGrid.GetAllTiles();
    }

    public GameObject GetTileAtCoord(int x, int y)
    {
        return combatGrid.GetTileAtCoord(x, y);
    }
}
