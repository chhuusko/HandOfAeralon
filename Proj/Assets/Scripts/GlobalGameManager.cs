using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    private static GlobalGameManager instance;
    [SerializeField] private int saveSlot;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public static GlobalGameManager GetInstance()
    {
        return instance;
    }
    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    
}


