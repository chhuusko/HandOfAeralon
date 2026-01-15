using UnityEngine;
using TMPro;
using FMODUnity;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class CombatVictoryScreenMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _info;
    [SerializeField] private GameObject _goToShopButton;
    [SerializeField] private GameObject _mainMenuButton;
    [SerializeField] private TMP_Text _mainMenuButtonText;

    public void SetTitle(string title)
    {
        _title.text = title;
    }

    public void SetScreenData(bool playerWon)
    {
        if (playerWon)
        { 
            SetWinScreen();
        }
        else
        {
            SetLoseScreen();
        }
    }

    public void SetWinScreen()
    {
        _goToShopButton.SetActive(true);
        _title.text = "Battle Won!";

        _info.text = GetSummaryInfo();

        _mainMenuButtonText.text = "Quit Game";

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerVictory, transform.position);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GoldGainAfterCombat, transform.position);
    }

    public void SetGameWonScreen()
    {
        _goToShopButton.SetActive(false);
        _title.text = "Congratulations!";

        _info.text = GetVictroyMessage() + GetSummaryInfo();

        _mainMenuButtonText.text = "Quit Game";
    }

    public void SetLoseScreen()
    {
        _goToShopButton.SetActive(false);
        _title.text = "You have been defeated!";

        _info.text = GetLoseMessage() + GetSummaryInfo();
        _mainMenuButtonText.text = "Quit Game";
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDefeated, transform.position);
    }

    public void PlayOneShotButtonClick()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick, transform.position);
    }

    private string GetVictroyMessage()
    {
        return "You have managed to defeat all the forces of Magor, well done!\n\n" +
                     "Now you can rest assured that the world will be safe and secure for future generations to come!\n\n";
    }

    private string GetLoseMessage()
    {
        return "You fought bravely against the forces of Magor, but in the end they proved too strong.\n\n" +
            "With your defeat, the world now stands on the brink of uncertainty, its future left unwritten.\n\n";

    }

    private string GetSummaryInfo()
    {
        return  "Level: " + SceneManager.GetActiveScene().name + "\n" +
                "Coins gained: " + GlobalGameManager.GetInstance().GetCombatCoins() + "\n" + 
                "Total enemies killed: " + GlobalGameManager.GetInstance().GetTotalEnemiesKilled() + "\n" +
                "Total heroes lost: " + GlobalGameManager.GetInstance().GetTotalHeroesLost() + "\n" +
                "Total battles won: " + GlobalGameManager.GetInstance().GetTotalBattlesWon();
    }


}


