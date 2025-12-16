using UnityEngine;
using TMPro;

public class CombatVictoryScreenMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _info;


    void Start()
    {
        CombatEventManager.OnEnterCombatStateEndCombat += SetScreenData;
    }

    private void OnDisable()
    {
        CombatEventManager.OnEnterCombatStateEndCombat -= SetScreenData;
    }

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

    private void SetWinScreen()
    {
        _title.text = "Battle Won!";
        _info.text = "Coins gained: " + GlobalGameManager.GetInstance().GetCombatCoins();
    }

    private void SetLoseScreen()
    {
        _title.text = "Battle Lost!";
        _info.text = "";
    }
}
