using UnityEngine;
using TMPro;

public class CombatVictoryScreenMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _info;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        _title.text = "You Win!";
        _info.text = "Coins gained: " + 200;
    }

    private void SetLoseScreen()
    {
        _title.text = "You Lose!";
        _info.text = "";
    }
}
