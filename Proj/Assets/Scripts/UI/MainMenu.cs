using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CanvasGroup mainMenu, saveMenu;
    [SerializeField] private GameObject[] saveSlot;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
    public void GoToSaveMenu()
    {
        mainMenu.alpha = 0;
        saveMenu.alpha = 1;
        mainMenu.gameObject.SetActive(false);
        saveMenu.gameObject.SetActive(true);
    }
    public void GoToMainMenu()
    {
        
        mainMenu.alpha = 1;
        saveMenu.alpha = 0;
        mainMenu.gameObject.SetActive(true);
        saveMenu.gameObject.SetActive(false);
    }
    public void SelectGameSlot(int slot)
    {
        GlobalGameManager.GetInstance().StartNewGame(slot);
    }
    public void LoadSaveMenuInfo()
    {
        // loads the info to the gameslot cards
    }
}
