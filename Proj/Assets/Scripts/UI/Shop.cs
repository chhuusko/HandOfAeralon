using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] private static Shop _instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public GameObject _mainCanvas, _overlayCanvas;
    [SerializeField] private GameObject _sellTab;
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private Transform[] _purchasCardPos;
    [SerializeField] private GameObject _purchaseCardPrefab;
    [SerializeField] private Transform[] _purchasCharacterPos;
    [SerializeField] private GameObject _purchaseCharacterPrefab;
    [SerializeField] private ClassDatabase _classDatabase;
    private List<Card> unlockedCards;
    private List<GameObject> _buyableItemInScene;
    
    private void FixedUpdate()
    {
        GlobalGameManager.GetInstance().ChangeCoins(1);
        UpdateMoneyUI();
    }
    public static Shop GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        _instance = this;
        _buyableItemInScene = new List<GameObject>();
        unlockedCards = CardsUnlocked.GetInstance().GetUnlockedCards();
        UpdateMoneyUI();
        LoadBuyCard();
        LoadBuyCharacter();

    }

    private void LoadBuyCharacter()
    {
        foreach (Transform t in _purchasCharacterPos)
        {
            GameObject newCharacterObject = Instantiate(_purchaseCharacterPrefab, t);
            newCharacterObject.GetComponent<BuyableCharacter>().SetCharacter(GetRandomCharacter());
            _buyableItemInScene.Add(newCharacterObject);
        }
    }
    public void LoadBuyCard()
    {
        foreach (Transform t in _purchasCardPos)
        {
            GameObject newCardObject = Instantiate(_purchaseCardPrefab, t);
            Card newCard = GetRandomUnlockedCard();
            newCardObject.GetComponent<CardUI>().SetUpUIElements(newCard);
            newCardObject.GetComponent<BuyableCard>().SetCard(newCard);
            _buyableItemInScene.Add(newCardObject);
        }
    }
    public Card GetRandomUnlockedCard()
    {
        return unlockedCards[Random.Range(0, unlockedCards.Count)];
    }
    public CharacterData GetRandomCharacter()
    {
        return new CharacterData(_classDatabase.Classes[Random.Range(0, 3)], Faction.Friendly, true);
    }
    
    public void OpenSellTab()
    {
        _sellTab.GetComponent<CardViewUI>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _sellTab.SetActive(true);
    }
    public void SellCard()
    {
        
    }
    public void Refresh()
    {
        foreach (GameObject item in _buyableItemInScene)
        {
            Destroy(item.gameObject);
        }
        _buyableItemInScene.Clear();
        LoadBuyCard();
        LoadBuyCharacter();
    }

    public void ExitShop()
    {
        LevelManager.GetInstance().StartNextLevel();
    }
    public void UpdateMoneyUI()
    {
        _balanceText.text =  GlobalGameManager.GetInstance().GetGameData().coins + "<voffset=25> <space=3> <sprite name=\"UI_icon_59\">";
    }

    public void ChangeCoins(int change)
    {
        GlobalGameManager.GetInstance().ChangeCoins(change);
        UpdateMoneyUI();
    }
}
