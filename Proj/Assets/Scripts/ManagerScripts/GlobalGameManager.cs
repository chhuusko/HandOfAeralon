using System.Collections.Generic;
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
    public List<Character> heroList;
    public List<Card> cardList;

    // misc
    public int reapersLedgerKills;


}
[CreateAssetMenu(fileName = "GlobalGameManager", menuName = "Manager/GlobalGameManager")]
public class GlobalGameManager : ScriptableObject
{
    [SerializeField] private DeckPreset _deckPreset;
    [SerializeField] private CharacterPrefabLibrary _characterLibrary;
    [SerializeField] private ClassDatabase _classDatabase;
    [SerializeField] private float _classTraitChance;
    private static GlobalGameManager _instance;
    private GameData _currentGame;

    [SerializeField] private int baseCoinReward = 200;
    [SerializeField] private int CoinRewardIncreasePerLevel = 50;


    [SerializeField] private bool startWithFullParty;
    public float ClassTraitChance => _classTraitChance;
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
        CombatEventManager.OnExitCombatStateEndCombat += GetCombatCoins;
    }
    private void OnDisable()
    {
        CombatEventManager.OnCharacterDeath -= RemoveCharacter;
        CombatEventManager.OnExitCombatStateEndCombat -= GetCombatCoins;
    }
    private void GetCombatCoins(bool playerWon)
    {
        _currentGame.coins += (baseCoinReward+(CoinRewardIncreasePerLevel*LevelManager.GetInstance().Getlevel()));
    }

    private void RemoveCharacter(Character obj)
    {
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
            GetTemp();
        }
        return _currentGame;
    }
    public void LoadGame(int slot)
    {
        //TODO
    }
    public void SaveGame(List<CharacterData> heroDataList, List<Character> heroList, List<Card> cardList, int level, int coins)
    {
        _currentGame.heroDataList = heroDataList;
        _currentGame.heroList = heroList;
        _currentGame.cardList = cardList;
        _currentGame.level = level;
        _currentGame.coins = coins;

    }
    public void StartNewGame(int slot)
    {
        GetTemp();
        LevelManager.GetInstance().StartNextLevel();
    }
    public void JSONWrite()
    {
        //TODO
    }
    /// <summary>
    /// Temporary function so that same data exist regardless of scene and order of scene load
    /// </summary>
    private void GetTemp()
    {
        _currentGame = new GameData();
        _currentGame.saveSlot = 1;
        _currentGame.seed = 67;
        LevelManager.GetInstance().GenerateMap(_currentGame.seed);
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


            _currentGame.cardList = new List<Card>(_deckPreset.GetCards());
        _currentGame.coins = 100;
        _currentGame.reapersLedgerKills = 0;
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


