using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private static Shop _instance;

    [SerializeField] public GameObject _mainCanvas, _overlayCanvas;
    [SerializeField] private GameObject _sellTab;
    [SerializeField] private GameObject _deckTab;

    [SerializeField] private TextMeshProUGUI _balanceText;

    [SerializeField] private Transform[] _purchasCardPos;
    [SerializeField] private GameObject _purchaseCardPrefab;
    [SerializeField] private Transform[] _purchasCharacterPos;
    [SerializeField] private GameObject _purchaseCharacterPrefab;

    [SerializeField] private ClassDatabase _classDatabase;

    [SerializeField] private Transform _partyHolder;
    [SerializeField] private GameObject _partyPortrait;

    [SerializeField] private TextMeshProUGUI _partyMembersText;

    [SerializeField] private TextMeshProUGUI _removeCardText;
    [SerializeField] private TextMeshProUGUI _refreshText;
    [SerializeField] private TextMeshProUGUI _healText;

    private List<Card> _unlockedCards;
    private List<GameObject> _buyableItemInScene;
    private List<GameObject> _partyPortraitInstances;

    //Costs
    [SerializeField] int _healPrice;
    [SerializeField] int _refreshPrice;
    [SerializeField] int _removeCardPrice;
    [SerializeField] int _addedRemoveCardPrice; 

    public static Shop GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        _removeCardText.text = "Remove card <color=yellow>"+_removeCardPrice+"</color><voffset=15><space=25><sprite name=\"UI_icon_59\">";
        _refreshText.text = "Refresh <color=Yellow>"+_refreshPrice+"</color><voffset=15><space=20><sprite name=\"UI_icon_59\">";
        _healText.text = "Heal Party (50%)\r\n<color=Yellow>"+ _healPrice+ "</color><voffset=15><space=20><sprite name=\"UI_icon_59\">";
        _instance = this;
        Application.targetFrameRate = -1;

        _buyableItemInScene = new List<GameObject>();
        _partyPortraitInstances = new List<GameObject>();
        _unlockedCards = CardsUnlocked.GetInstance().GetUnlockedCards();

        UpdateMoneyUI();
        LoadParty();
        LoadBuyCard();
        LoadBuyCharacter();
    }

    public void LoadParty()
    {
        if (_partyPortraitInstances.Count > 0)
        {
            foreach (GameObject partyMembers in _partyPortraitInstances)
            {
                Destroy(partyMembers);
            }
            _partyPortraitInstances.Clear();
        }
        foreach (CharacterData character in GlobalGameManager.GetInstance().GetGameData().heroDataList)
        {
            GameObject newC = Instantiate(_partyPortrait, _partyHolder);
            newC.GetComponent<PartyMemberUI>().SetUIElements(character);
            _partyPortraitInstances.Add(newC);
        }
        _partyMembersText.text = "Party (" + GlobalGameManager.GetInstance().GetGameData().heroDataList.Count + "/4)";

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
        return _unlockedCards[Random.Range(0, _unlockedCards.Count)];
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
    public void OpenDeckTab()
    {
        _deckTab.GetComponent<CardViewUI>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _deckTab.SetActive(true);
    }
    public void SellCard()
    {
        
    }
    public void Refresh()
    {
        if (CanAfford(_refreshPrice))
        {
            foreach (GameObject item in _buyableItemInScene)
            {
                Destroy(item.gameObject);
            }
            _buyableItemInScene.Clear();
            LoadBuyCard();
            LoadBuyCharacter();
            Bought(_refreshPrice);
        }
        
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
    public void Heal()
    {
        if (CanAfford(_healPrice))
        {
            List<CharacterData> heroList = GlobalGameManager.GetInstance().GetGameData().heroDataList;
            foreach(CharacterData character in heroList)
            {
                Debug.Log((int)(character.BaseHealthPoints * 0.5f) + "healed.");
                character.Heal( (int)(character.BaseHealthPoints*0.5f));
                Debug.Log(character.BaseHealthPoints + "current.");
            }
            Bought(_healPrice);
        }
        LoadParty();
    }
    public bool CanAfford(int cost)
    {
        return GlobalGameManager.GetInstance().GetGameData().coins >= cost;
    }
    public void Bought(int cost)
    {
        GlobalGameManager.GetInstance().ChangeCoins(-cost);
        UpdateMoneyUI();
    }
    public int GetRemoveCardPrice()
    {
        return _removeCardPrice;
    }
}
