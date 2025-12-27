using UnityEngine;

public class AI_Executor : MonoBehaviour
{
    // Singleton pattern
    private static AI_Executor Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public static AI_Executor GetInstance()
    {
        return Instance;
    }
    // End of singleton pattern
}
