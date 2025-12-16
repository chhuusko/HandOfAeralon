using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(fileName = "LevelManager", menuName = "Manager/LevelManager")]
public class LevelManager : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<string> tutorialCombatList;
    [SerializeField] private List<string> easyCombatList;
    [SerializeField] private List<string> mediumCombatList;
    [SerializeField] private List<string> hardCombatList;

    private static LevelManager _instance;
    private string[] _combatList;
    private string[] _generatedList;
    private int _level = 0;
    //private int _gameLevels = 10;
    private int _difficulty = 0;

    [SerializeField] private float statIncreaseFactor = 1.2f;
    [SerializeField] public int statIncreaseInterval = 3;
    public float statIncrease { get; private set; }

    [SerializeField] private float enemyStatIncreaseFactor = 1.2f;
    [SerializeField] public int enemyStatIncreaseInterval = 1;
    public float enemyStatIncrease { get; private set; }

    private int menuFPSCap = 60;
    private CombatGrid _combatGrid;
    private bool _isTutorialCompleted;

    public static LevelManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<LevelManager>("LevelManager");
            _instance._level = 0;
            _instance._isTutorialCompleted = false;
            LevelManager.Initialize();
        }
        return _instance;
    }
    public static void Initialize()
    {
        _instance._level = 0;
        _instance._isTutorialCompleted = false;
        _instance.statIncrease = 1;
        _instance.enemyStatIncrease = 1;
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
        TieredRandomLevel();
    }
    private void StaticLevel()
    {
        
        Debug.Log(_level + " level");
        if (SceneManager.GetActiveScene().name == "ShopScene" || _level == 0)
        {
            if (_level >= easyCombatList.Count) _level = 0;
            SceneManager.LoadScene(easyCombatList[_level]);
            _level++;

            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            SceneManager.LoadScene("ShopScene");
            Application.targetFrameRate = menuFPSCap;
            QualitySettings.vSyncCount = 0;
        }
    }
    private void IncreaseStat()
    {
        if (_level % statIncreaseInterval == 0)
        {
            Debug.Log("PLAYERSTATSUPPDATED----------------------------------");
            foreach (CharacterData character in GlobalGameManager.GetInstance().GetGameData().heroDataList)
            {
                character.CalculateDerivedStats((statIncreaseFactor * (_level / statIncreaseInterval)));
               // Debug.Log("deriveddamage: " + character.DerivedDamage + " base damage: " + character.BaseDamage); 
            }
        } 
        /*
        if (_level % enemyStatIncreaseInterval == 0)
        {
            foreach (Character character in CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Enemy))
            {
                character.Data.CalculateDerivedStats((statIncrease * (_level / statIncreaseInterval)));
                // Debug.Log("deriveddamage: " + character.DerivedDamage + " base damage: " + character.BaseDamage); 
            }
        }
        */
       
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("MainMenu");
        _level = 0;
        _difficulty = 0;
        _isTutorialCompleted = false;
    }
    private void TieredRandomLevel()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene" || SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (_level != 0)
            {
                IncreaseStat();
            }
            
            //loadCombat
            if (_isTutorialCompleted)
            {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 1;
                _difficulty = _level / 5;
                switch (_difficulty)
                {
                    case 0:
                        SceneManager.LoadScene(easyCombatList[Random.Range(0, easyCombatList.Count)]);
                        break;
                    case 1:
                        SceneManager.LoadScene(mediumCombatList[Random.Range(0, mediumCombatList.Count)]);
                        break;
                    default:
                        SceneManager.LoadScene(hardCombatList[Random.Range(0, hardCombatList.Count)]);
                        break;
                }
                _level++;
            }
            else
            {
                TutorialLevel();
            }
        }
        else
        {
            Application.targetFrameRate = menuFPSCap;
            QualitySettings.vSyncCount = 0;
            SceneManager.LoadScene("ShopScene");
        }
    }
    private void TutorialLevel()
    {
        SceneManager.LoadScene(tutorialCombatList[_level]);
        
        _level++;
        if (_level == tutorialCombatList.Count)
        {
            _isTutorialCompleted = true;
        }
    }
    public int Getlevel()
    {
        return _level;
    }
    public CombatGrid GetCombatLevel()
    {
        string filePathToload = Application.dataPath + "\\JSON BattleGrids\\" + _generatedList[_level] + ".json";

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
