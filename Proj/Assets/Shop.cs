using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ExitShop()
    {
        LevelManager.GetInstance().StartNextLevel();
    }
}
