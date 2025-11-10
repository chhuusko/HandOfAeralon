using UnityEngine;
using UnityEngine.SceneManagement;

public struct GameData
{
    public int saveSlot;
    public string playTime;
    public int seed;
    int seedInfo; // seedInfo current nod index
    // heroList
    // cardList
    int coins;
    // other
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
        
    }
    public void SaveGame()
    {
        // Update seedInfo etc

    }
    public void StartNewGame(int slot)
    {
        _currentGame = new GameData();
        _currentGame.saveSlot = slot;
        _currentGame.seed = Random.Range(0, 1000);
        SceneManager.LoadScene(1);
    }
    public void JSONWrite()
    {

    }
}


