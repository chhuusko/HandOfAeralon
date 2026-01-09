using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatIncreasePanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statText;
    private void Awake()
    {
        Showpanel(LevelManager.GetInstance().IncreaseStat());
    }
    public void Showpanel(bool b)
    {
        panel.SetActive(b);
        int formatedstat = Mathf.RoundToInt((LevelManager.GetInstance().statIncrease - 1f) * 100f);
        levelText.text = "" + LevelManager.GetInstance().GetStatLevel();
        statText.text = "Health +" + formatedstat + "%" + "\nDamage +" + formatedstat + "%";
    }

}
