using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct GameData
{
    public int saveSlot;
    public int seed;
    public int level; 
    public int coins;

    public List<Character> heroList;
    public List<Card> cardList;

}

public class GlobalGameManager : MonoBehaviour
{
    [SerializeField] private DeckPreset _deckPreset;
    private LevelManager _levelManager;
    private static GlobalGameManager _instance;
    private GameData _currentGame;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _levelManager = GetComponent<LevelManager>();
        DontDestroyOnLoad(this.gameObject);
    }
    public static GlobalGameManager GetInstance()
    {
        return _instance;
    }
    public GameData GetGameData()
    {
        return _currentGame;
    }
    public void LoadGame(int slot)
    {
        //TODO
    }
    public void SaveGame(List<Character> heroList, List<Card> cardList, int level, int coins)
    {
        _currentGame.heroList = heroList;
        _currentGame.cardList = cardList;
        _currentGame.level = level;
        _currentGame.coins = coins;

    }
    public void StartNewGame(int slot)
    {
        _currentGame = new GameData();
        _currentGame.saveSlot = slot;
        _currentGame.seed = Random.Range(0, 1000);
        _levelManager.GenerateMap(_currentGame.seed);
        _currentGame.heroList = new List<Character>();
        _currentGame.cardList = new List<Card>(_deckPreset.GetCards());
        _currentGame.coins = 50;
        SceneManager.LoadScene("ShopScene"); //TODO
    }
    public void JSONWrite()
    {
        //TODO
    }
}


