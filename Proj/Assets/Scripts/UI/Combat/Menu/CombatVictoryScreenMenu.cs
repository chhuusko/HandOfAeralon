using UnityEngine;
using TMPro;
using FMODUnity;

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
        string summaryInfo = "Coins gained: " + GlobalGameManager.GetInstance().GetCombatCoins() + "\n"
                             + "Total enemies killed: " + GlobalGameManager.GetInstance().GetTotalEnemiesKilled() + "\n"
                             + "Total heroes lost: " + GlobalGameManager.GetInstance().GetTotalHeroesLost() + "\n"
                             + "Total battles won: " + GlobalGameManager.GetInstance().GetTotalBattlesWon();
        _info.text = summaryInfo;
        _mainMenuButtonText.text = "Quit Game";
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerVictory, transform.position);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GoldGainAfterCombat, transform.position);
    }

    public void SetGameWonScreen()
    {
        _goToShopButton.SetActive(false);
        _title.text = "Congratulations!";

        string victoryMessage = "You have managed to defeat all the forces of Magor, well done!\n\n" +
                     "Now you can rest assured that the world will be safe and secure for future generations to come!\n\n";

        string summaryInfo = "Coins gained: " + GlobalGameManager.GetInstance().GetCombatCoins() + "\n"
                             + "Total enemies killed: " + GlobalGameManager.GetInstance().GetTotalEnemiesKilled() + "\n"
                             + "Total heroes lost: " + GlobalGameManager.GetInstance().GetTotalHeroesLost() + "\n"
                             + "Total battles won: " + GlobalGameManager.GetInstance().GetTotalBattlesWon();

        _info.text = victoryMessage + summaryInfo;
        _mainMenuButtonText.text = "Quit Game";
    }

    public void SetLoseScreen()
    {
        _goToShopButton.SetActive(false);
        _title.text = "Battle Lost!";
        string summaryInfo = "Total enemies killed: " + GlobalGameManager.GetInstance().GetTotalEnemiesKilled() + "\n"
                             + "Total heroes lost: " + GlobalGameManager.GetInstance().GetTotalHeroesLost() + "\n"
                             + "Total battles won: " + GlobalGameManager.GetInstance().GetTotalBattlesWon();
        _info.text = summaryInfo;
        _mainMenuButtonText.text = "Quit Game";
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDefeated, transform.position);
    }

    public void PlayOneShotButtonClick()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick, transform.position);
    }
}
