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

}
[CreateAssetMenu(fileName = "GlobalGameManager", menuName = "Manager/GlobalGameManager")]
public class GlobalGameManager : ScriptableObject
{
    [SerializeField] private DeckPreset _deckPreset;
    [SerializeField] private CharacterPrefabLibrary _characterLibrary;
    [SerializeField] private ClassDatabase _classDatabase;
    private static GlobalGameManager _instance;
    private GameData _currentGame;
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
    private void GetCombatCoins()
    {
        _currentGame.coins += 100;
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
        SceneManager.LoadScene("Graveyard12x10_Easy"); //TODO
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
        
        _currentGame.heroList = new List<Character>
        {
            _characterLibrary.GetPrefab(CharacterClass.Barbarian).GetComponent<Character>(),
            _characterLibrary.GetPrefab(CharacterClass.Sorceress).GetComponent<Character>(),
            _characterLibrary.GetPrefab(CharacterClass.Rogue).GetComponent<Character>(),
            _characterLibrary.GetPrefab(CharacterClass.Bard).GetComponent<Character>()
        };
        _currentGame.heroDataList = new List<CharacterData>(){
            new CharacterData(_classDatabase.Classes[(int)CharacterClass.Barbarian], Faction.Friendly, true),
            new CharacterData(_classDatabase.Classes[(int)CharacterClass.Rogue], Faction.Friendly, true),
            new CharacterData(_classDatabase.Classes[(int)CharacterClass.Bard], Faction.Friendly, true),
            new CharacterData(_classDatabase.Classes[(int)CharacterClass.Sorceress], Faction.Friendly, true)
        };

        _currentGame.cardList = new List<Card>(_deckPreset.GetCards());
        Debug.Log(_currentGame.cardList.Count);
        _currentGame.coins = 100;
    }
    public void SaveCards(List<Card> cards)
    {
        _currentGame.cardList = cards;
    }
    public void ChangeCoins(int amount)
    {
        _currentGame.coins += amount;
    }
}


