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
    private static GlobalGameManager instance;
    private GameData currentGame;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public static GlobalGameManager GetInstance()
    {
        return instance;
    }
    public void ChangeScene(int sceneIndex)
    {
        Debug.Log("Loaded" + SceneManager.GetSceneByBuildIndex(sceneIndex).name);
        SceneManager.LoadScene(sceneIndex);
        
    }
    public void StartNewGame(int slot)
    {
        currentGame = new GameData();
        currentGame.saveSlot = slot;
        currentGame.seed = Random.Range(0, 1000);
    }
    public void LoadGame(int slot)
    {

    }
    
}


