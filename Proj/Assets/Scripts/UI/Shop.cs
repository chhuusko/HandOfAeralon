using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private static Shop _instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject _sellTab;
    [SerializeField] private Transform[] _purchasCardPos;
    [SerializeField] private GameObject _purchaseCardPrefab;
    private List<Card> unlockedCards;
    private List<GameObject> _buyableCardInScene;
    private void Awake()
    {
        _buyableCardInScene = new List<GameObject>();
        unlockedCards = CardsUnlocked.GetInstance().GetUnlockedCards();
        LoadBuyCard();
    }
    public static Shop GetInstance()
    {
        return _instance;
    }
    public void OpenSellTab()
    {
        _sellTab.GetComponent<CardViewUI>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _sellTab.SetActive(true);
    }
    public void LoadBuyCard()
    {
        foreach (Transform t in _purchasCardPos)
        {
            GameObject newCardObject = Instantiate(_purchaseCardPrefab, t);
            newCardObject.GetComponent<BuyableCard>().SetCard(GetRandomUnlockedCard());
            _buyableCardInScene.Add(newCardObject);
        }
    }
    public Card GetRandomUnlockedCard()
    {
        return unlockedCards[Random.Range(0, unlockedCards.Count)];
    }
    public void Refresh()
    {
        foreach (GameObject t in _buyableCardInScene)
        {
            Destroy(t.gameObject);
        }
        _buyableCardInScene.Clear();
        LoadBuyCard();
    }
    public void SellCard()
    {
        
    }
    public void ExitShop()
    {
        LevelManager.GetInstance().StartNextLevel();
    }
}
