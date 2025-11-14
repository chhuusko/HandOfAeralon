using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject _sellTab;
    [SerializeField] private static Shop _instance;

    public static Shop GetInstance()
    {
        return _instance;
    }
    public void OpenSellTab()
    {
        _sellTab.GetComponent<CardViewUI>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _sellTab.SetActive(true);
    }
    public void SellCard()
    {
        
    }
    public void ExitShop()
    {
        LevelManager.GetInstance().StartNextLevel();
    }
}
