using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public struct GameData
{
    public int saveSlot;
    public int seed;
    public int level; 
    public int coins;

    public List<CharacterData> heroDataList;
    public List<TraitManager> traitManagerDataList;
    public List<Card> cardList;

    // misc
    public int reapersLedgerKills;
    public int totalEnemiesKilled;
    public int totalHeroesLost;
    public int totalBattlesWon;
}

[CreateAssetMenu(fileName = "GlobalGameManager", menuName = "Manager/GlobalGameManager")]
public class GlobalGameManager : ScriptableObject
{
    [SerializeField] private DeckPreset _deckPreset;
    [SerializeField] private CharacterPrefabLibrary _characterLibrary;
    [SerializeField] private ClassDatabase _classDatabase;
    [SerializeField, Range(0, 100)] private float classTraitChancePercent;
    private static GlobalGameManager _instance;
    private GameData _currentGame;
    [SerializeField] private int startCoins = 100;
    [SerializeField] private int baseCoinReward = 200;
    [SerializeField] private int CoinRewardIncreasePerLevel = 50;


    [SerializeField] private bool startWithFullParty;
    public float ClassTraitChancePercent => classTraitChancePercent;
    public static GlobalGameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<GlobalGameManager>("GlobalGameManager");
        }
        return _instance;
    }
    private void OnEnable()
    {
        CombatEventManager.OnCharacterDeath += RemoveCharacter;
    }
    private void OnDisable()
    {
        CombatEventManager.OnCharacterDeath -= RemoveCharacter;
    }
    public int GetCombatCoins()
    {
        int level = LevelManager.GetInstance().Getlevel();

        int increaseSteps = level / 3;

        int combatCoins = baseCoinReward + (CoinRewardIncreasePerLevel * increaseSteps);
        _currentGame.coins += combatCoins;

        return combatCoins;
    }

    private void RemoveCharacter(Character obj)
    {
        if(obj.GetFaction() == Faction.Enemy)
            _currentGame.totalEnemiesKilled++;
        else if (obj.GetFaction() == Faction.Friendly)
            _currentGame.totalHeroesLost++;

        Dictionary<CharacterData, Character> dict = CombatManager._instance.GetCharacterDataDict();
        foreach (var pair in dict)
        {
            if (pair.Value == obj)
            {
                _currentGame.heroDataList.Remove(pair.Key);
                break;
            }
        }
    }

    public GameData GetGameData()
    {
        if (_currentGame.cardList == null)
        {
            GetTemp(0);
        }
        return _currentGame;
    }

    public int GetTotalEnemiesKilled() { return _currentGame.totalEnemiesKilled; }
    public int GetTotalHeroesLost() { return _currentGame.totalHeroesLost; }
    public int GetTotalBattlesWon() { return _currentGame.totalBattlesWon; }
    public void SetTotalBattlesWon(int battlesWon) { _currentGame.totalBattlesWon = battlesWon; }
    public void IncrementTotalBattlesWon() { _currentGame.totalBattlesWon++; }
    
    public void LoadGame(int slot)
    {
        string path = GetSaveSlotPath(slot);
        string json = File.ReadAllText(path);
        _currentGame = JsonUtility.FromJson<GameData>(json);
    }
    public void SaveGame(List<CharacterData> heroDataList, List<Card> cardList, int level, int coins)
    {
        _currentGame.heroDataList = heroDataList;
        _currentGame.cardList = cardList;
        _currentGame.level = level;
        _currentGame.coins = coins;

    }
    private void SelectSlot(int slot)
    {
        _currentGame = new GameData();
        if (File.Exists(GetSaveSlotPath(slot)))
        {
            
            Debug.Log("Slot Load");
            LoadGame(slot);
            
        }
        else
        {
            Debug.Log("Slot Created");
            CreateGameSave(slot);
            
        }
    }
    public void RemoveSlot(int slot)
    {
        if (File.Exists(GetSaveSlotPath(slot)))
        {
            File.Delete(GetSaveSlotPath(slot));
        }
    }
    public void StartNewGame(int slot)
    {
        GetTemp(0);
        LevelManager.GetInstance().StartNextLevel();
        
    }
    private string GetSaveSlotPath(int slot)
    {
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string path = Path.Combine(documentsPath, "HandOfAeralon", $"gameSave{slot}.json");
        Debug.Log("Path " + path);
        return path;
    }
    public void Save()
    {
        string path = GetSaveSlotPath(_currentGame.saveSlot);
        string json = JsonUtility.ToJson(_currentGame);
        File.WriteAllText(path, json);
    }
    public void CreateGameSave(int slot)
    {
        GetTemp(slot);
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        if (!Directory.Exists(Path.Combine(documentsPath, "HandOfAeralon")))
        {
            Directory.CreateDirectory(Path.Combine(documentsPath, "HandOfAeralon")); 
        }
        Save();
            
    }
    public void CreateGameFolder()
    {

    }

    /// <summary>
    /// Temporary function so that same data exist regardless of scene and order of scene load
    /// </summary>
    private void GetTemp(int slot)
    {
        _currentGame = new GameData();
        _currentGame.saveSlot = slot;
        _currentGame.seed = 67;
        LevelManager.GetInstance().GenerateMap(_currentGame.seed);
        GenerateParty();
        _currentGame.cardList = new List<Card>(_deckPreset.GetCards());
        _currentGame.coins = startCoins;
        _currentGame.reapersLedgerKills = 0;
    }
    private void GenerateParty()
    {
        if (startWithFullParty)
        {
            _currentGame.heroDataList = new List<CharacterData>(){
                new CharacterData(_classDatabase.Classes[(int)CharacterClass.Barbarian], Faction.Friendly, true),
                new CharacterData(_classDatabase.Classes[(int)CharacterClass.Rogue], Faction.Friendly, true),
                new CharacterData(_classDatabase.Classes[(int)CharacterClass.Bard], Faction.Friendly, true),
                new CharacterData(_classDatabase.Classes[(int)CharacterClass.Sorceress], Faction.Friendly, true)
            };
        }
        else
        {
            _currentGame.heroDataList = new List<CharacterData>(){
                new CharacterData(_classDatabase.Classes[(int)CharacterClass.Barbarian], Faction.Friendly, true)
            };
        }

        HashSet<string> usedNames = new HashSet<string>();
        foreach (var character in _currentGame.heroDataList)
        {
            string name = CharacterNameGenerator.GenerateName(character.ClassData, usedNames);
            usedNames.Add(name);
            character.SetName(name);
        }
    }
    public void SaveCards(List<Card> cards)
    {
        _currentGame.cardList = cards;
    }
    public void ChangeCoins(int amount)
    {
        _currentGame.coins += amount;
    }
    public void ReapersLedgerKillChange(int change)
    {
        _currentGame.reapersLedgerKills += change;
    }
}


