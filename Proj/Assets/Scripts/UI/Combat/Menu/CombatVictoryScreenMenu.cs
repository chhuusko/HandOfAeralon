using UnityEngine;
using TMPro;

public class CombatVictoryScreenMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _info;
    [SerializeField] private GameObject _goToShopButton;

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
        _info.text = "Coins gained: " + GlobalGameManager.GetInstance().GetCombatCoins();
    }

    public void SetLoseScreen()
    {
        _goToShopButton.SetActive(false);
        _title.text = "Battle Lost!";
        _info.text = "";
    }
}
