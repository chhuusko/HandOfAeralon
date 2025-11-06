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
    List<CombatGridTile> tiles = new List<CombatGridTile>();

    public void AddTile(CombatGridTileData tileData)
    {
        CombatGridTile tile = new CombatGridTile(tileData);
        tiles.Add(tile);
    }
}

public class CombatManager : MonoBehaviour
{
    [SerializeField] private CombatState combatState;
    [SerializeField] private CombatTurn currentTurn;

    [SerializeField] private bool combatGridLoaded = false;
    [SerializeField] private CombatGrid combatGrid;

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

        CombatGridTileSerializedSaveData tileData = JsonUtility.FromJson<CombatGridTileSerializedSaveData>(jsonFileData);
        for(int i = 0; i < tileData.tileData.Count; i++)
        {
            Debug.Log("TileType : " + tileData.tileData[i].GetTileType() + 
                      "\nTilePosition: " + tileData.tileData[i].GetTilePosition());

            combatGrid.AddTile(tileData.tileData[i]);
            
        }
        
    }

    private void EvaluateInitiativeOrder()
    {

    }
}
