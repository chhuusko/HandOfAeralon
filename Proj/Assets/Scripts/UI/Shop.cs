using FMODUnity;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private static Shop _instance;

    [SerializeField] public GameObject _mainCanvas, _overlayCanvas;
    [SerializeField] private GameObject _sellTab;
    [SerializeField] private GameObject _deckTab;
    [SerializeField] private CardPackViewUIShop _cardpackTab;
    [SerializeField] private TextMeshProUGUI _balanceText;

    [SerializeField] private Transform[] _purchasCardPos;
    [SerializeField] private GameObject _purchaseCardPrefab;
    [SerializeField] private GameObject _purchaseCardPackPrefab;
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

    [SerializeField] private EventReference bougtSound, errorSound, healSound;
    //Costs
    [SerializeField] int _healPrice;
    [SerializeField] int _addedHealPrice;
    [SerializeField] int _refreshPrice;
    [SerializeField] int _removeCardPrice;
    [SerializeField] int _addedRemoveCardPrice; 

    public static System.Action onSellCard;

    [SerializeField] private TextMeshProUGUI _nextCombatText; // JLW
    public static Shop GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (GlobalGameManager.GetInstance().GetTotalBattlesWon() <= 1)
        {
            _healPrice = 0;
        }
        _removeCardText.text = "Balance <color=yellow>" + GlobalGameManager.GetInstance().GetGameData().coins + "</color><voffset=20><space=40><sprite name=\"UI_icon_59\">";
        _refreshText.text = "Refresh <color=Yellow>"+_refreshPrice+"</color><voffset=15><space=20><sprite name=\"UI_icon_59\">";
        _healText.text = "Heal Party (50%)\r\n<color=Yellow>"+ _healPrice+ "</color><voffset=15><space=20><sprite name=\"UI_icon_59\">";
        _instance = this;

        _buyableItemInScene = new List<GameObject>();
        _partyPortraitInstances = new List<GameObject>();
        _unlockedCards = CardsUnlocked.GetInstance().GetUnlockedCards();

        UpdateMoneyUI();
        LoadParty();
        LoadBuyCard();
        LoadBuyCharacter();
        UpdateNextCombatText();
    }
    
    public void SoldCard() {
        _removeCardPrice += _addedRemoveCardPrice;
        //_removeCardText.text = "Hold to Remove Card <color=yellow>" + _removeCardPrice + "</color><voffset=20><space=40><sprite name=\"UI_icon_59\">";
        _removeCardText.text = "Balance <color=yellow>" + GlobalGameManager.GetInstance().GetGameData().coins + "</color><voffset=20><space=40><sprite name=\"UI_icon_59\">";
        onSellCard?.Invoke(); 
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
        // Get already used names.
        HashSet<string> usedNames = new HashSet<string>(
            GlobalGameManager.GetInstance().GetGameData().heroDataList.Select(h => h.Name));
        
        foreach (Transform t in _purchasCharacterPos)
        {
            GameObject newCharacterObject = Instantiate(_purchaseCharacterPrefab, t);
            CharacterData newCharacter = GetRandomCharacter();
            newCharacter.RecalculateLevelScaling(LevelManager.GetInstance().statIncrease);
            newCharacter.SetCurrentHealthPoints(newCharacter.DerivedHealthPoints);
            newCharacterObject.GetComponent<BuyableCharacter>().SetCharacter(newCharacter);
            _buyableItemInScene.Add(newCharacterObject);
            
            string name = CharacterNameGenerator.GenerateName(newCharacter.ClassData, usedNames);
            newCharacter.SetName(name);
            usedNames.Add(name);
        }
    }
    public void LoadBuyCard()
    {
        for (int i = 0; i < _purchasCardPos.Length; i++)
        {
            if (i != _purchasCharacterPos.Length - 1)
            {
                GameObject newCardObject = Instantiate(_purchaseCardPrefab, _purchasCardPos[i]);
                Card newCard = GetRandomUnlockedCard();
                newCardObject.GetComponent<CardUI>().SetUpUIElements(newCard);
                newCardObject.GetComponent<BuyableCard>().SetCard(newCard);
                _buyableItemInScene.Add(newCardObject);
            }
            else
            {
                GameObject newCardObject = Instantiate(_purchaseCardPackPrefab, _purchasCardPos[i]);
                _buyableItemInScene.Add(newCardObject);
            }
            
        }
    }

    private void UpdateNextCombatText()
    {
        _nextCombatText.text = $"Go to Level {LevelManager.GetInstance().Getlevel() +1}";
    }

    public Card GetRandomUnlockedCard()
    {
        return _unlockedCards[Random.Range(0, _unlockedCards.Count)];
    }
    public CharacterData GetRandomCharacter()
    {
        return new CharacterData(_classDatabase.Classes[Random.Range(0, 4)], Faction.Friendly, true);
    }
    
    public void OpenSellTab()
    {
        //Que
        _sellTab.GetComponent<CardViewUIShop>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _sellTab.GetComponent<CardViewUIShop>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _sellTab.SetActive(true);
    }
    public void OpenDeckTab()
    {
        _deckTab.GetComponent<CardViewUI>().UpdateCards(GlobalGameManager.GetInstance().GetGameData().cardList);
        _deckTab.SetActive(true);
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
        _removeCardText.text = "Balance <color=yellow>" + GlobalGameManager.GetInstance().GetGameData().coins + "</color><voffset=20><space=40><sprite name=\"UI_icon_59\">";
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
                character.Heal( (int)(character.BaseHealthPoints*0.5f));
            }
            AudioManager.Instance.PlayOneShot(healSound, transform.position);
           
            
            Bought(_healPrice);
            _healPrice += _addedHealPrice;
            _healText.text = "Heal Party (50%)\r\n<color=Yellow>" + _healPrice + "</color><voffset=15><space=20><sprite name=\"UI_icon_59\">";
        }
        LoadParty();
    }
    public static bool CanAfford(int cost)
    {
        return GlobalGameManager.GetInstance().GetGameData().coins >= cost;
    }
    public void Bought(int cost)
    {
        GlobalGameManager.GetInstance().ChangeCoins(-cost);
        AudioManager.Instance.PlayOneShot(bougtSound, transform.position);
        Debug.Log(GlobalGameManager.GetInstance().GetGameData().coins);
        UpdateMoneyUI();
    }
    public int GetRemoveCardPrice()
    {
        return _removeCardPrice;
    }
    public CardPackViewUIShop GetCardPack()
    {
        return _cardpackTab;
    }
}
