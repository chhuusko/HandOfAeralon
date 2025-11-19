using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "LevelManager", menuName = "Manager/LevelManager")]
public class LevelManager : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static LevelManager _instance;
    private string[] _combatList;
    private string[] _generatedList;
    private int level = 0;
    private int gameLevels = 10;

    private CombatGrid _combatGrid;
    public static LevelManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<LevelManager>("LevelManager");
        }
        return _instance;
    }
    private void Awake()
    {
        //_instance = this;
        //_combatList = Directory.GetFiles("Assets/JSON BattleGrids").Where(f => !f.EndsWith(".meta")).ToArray(); 
        //_generatedList = new string[10];
    }
    public void GenerateMap(int seed)
    {
        //Random.InitState(seed);
        //for (int i = 0; i < gameLevels; i++)
        //{
        //     _generatedList[i] = _combatList[Random.Range(0,_combatList.Length)];
        //    Debug.Log(_generatedList[i]);
        //}
    }
    public void StartNextLevel() 
    {
        level++;
        if (level % 2 == 0)
        {
            //GetCombatLevel();
            SceneManager.LoadScene("Graveyard12x10_Easy");
        }
        else
        {
            SceneManager.LoadScene("ShopScene");
        }
        
        
    }
    public CombatGrid GetCombatLevel()
    {
        string filePathToload = Application.dataPath + "\\JSON BattleGrids\\" + _generatedList[level] + ".json";

        if (!System.IO.File.Exists(filePathToload))
        {
            Debug.Log("Level File didn't exist or filepath was wrong!");
            return null;
        }

        string jsonFileData = System.IO.File.ReadAllText(filePathToload);
        if (jsonFileData.Length == 0)
        {
            Debug.Log("json File Data was empty!");
            return null;
        }

        CombatGridSerializedSaveData combatGrid = JsonUtility.FromJson<CombatGridSerializedSaveData>(jsonFileData);

        _combatGrid.SetCombatGridSize(combatGrid._gridWidth, combatGrid._gridHeight);
        _combatGrid.SetTileSize(combatGrid._tileSize);
        Debug.Log("CombatGrid tileSize: " + combatGrid._tileSize);

        for (int i = 0; i < combatGrid._tileData.Count; i++)
        {
            Debug.Log("tiled[" + i + "]: " + "\tTileType : " + combatGrid._tileData[i].GetTileType() +
                      "\tTileIndex: " + combatGrid._tileData[i].GetTilePosition() + "\n");

            _combatGrid.AddTile(combatGrid._tileData[i]);
        }

        for (int i = 0; i < combatGrid._characterData.Count; i++)
        {
            _combatGrid.AddCharacter(combatGrid._characterData[i]);
        }
        return _combatGrid;
    }
}
