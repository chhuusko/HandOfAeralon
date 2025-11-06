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
public class TilePrefabMap
{
    public TileType tileType;
    public GameObject tilePrefab;
};

[System.Serializable]
public class CombatGrid
{
    [SerializeField] private TilePrefabMap[] tilePrefabMap;
    CombatGridTile[,] tiles;
    public int _width { get; private set; }
    public int _height { get; private set; }
    public void SetCombatGridSize(int w, int h)
    {
        _width  = w;
        _height = h;
        tiles   = new CombatGridTile[w, h];
    } 

    // TODO (Calle): Använd TilePrefabLibrary för att skapa 
    // mappningar mellan TileType och Tile GO Prefabs.
    public void AddTile(CombatGridTileData tileData)
    {
        CombatGridTile tile = new CombatGridTile(tileData);
        Vector2 position = tileData.GetTilePosition();
        tiles[(int)position.x, (int)position.y] = tile;

        switch(tileData.GetTileType())
        {
            case TileType.Walkable:

                break;

        }

    }
}

public class CombatManager : MonoBehaviour
{
    [SerializeField] private CombatState combatState;
    [SerializeField] private CombatTurn currentTurn;

    [SerializeField] private CombatGrid combatGrid;
    [SerializeField] private bool combatGridLoaded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatState = CombatState.IntroCinematic;
    }

    // Update is called once per frame
    void Update()
    {
        switch(combatState)
        {
            case CombatState.IntroCinematic:
                {
                    HandleIntroCinematic();
                } break;
            case CombatState.LoadCombatLevel:
                {
                    HandleLoadCombatLevel();
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

    }

    private void HandleLoadCombatLevel()
    {
        if(!combatGridLoaded)
        {
            combatGridLoaded = true;
            LoadNextLevel();
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
        string fileName = "TileData";
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
}
