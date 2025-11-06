using UnityEngine;
using UnityEngine.SceneManagement;

public struct GameData
{
    int saveSlot;
    string playTime;
    int seed;
    // seedInfo
    // heroList
    // cardList
    int coins;
    // other
}

public class GlobalGameManager : MonoBehaviour
{
    private static GlobalGameManager instance;

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
    
    
}


