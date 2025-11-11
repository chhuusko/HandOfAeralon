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
    private static GlobalGameManager _instance;
    private GameData _currentGame;
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public static GlobalGameManager GetInstance()
    {
        return _instance;
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
        _currentGame.coins = 0;
        SceneManager.LoadScene(1); //TODO
    }
    public void JSONWrite()
    {
        //TODO
    }
}


