using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CanvasGroup mainMenu, saveMenu;
    [SerializeField] private GameObject[] saveSlot;

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
    public void LoadSaveMenuInfo()
    {
        // loads the info to the gameslot cards
    }
}
