using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "CardsUnlocked", menuName = "Item/CardsUnlocked", order = 1)]
public class CardsUnlocked : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static CardsUnlocked _instance;

    [Header("Info")]
    [SerializeField] private CardList allCards;
    [SerializeField] private CardList defaultUnlocked;
    [SerializeField] private CardsUnlockedStatus[] cardsUnlocked;

    [System.Serializable]
    public struct CardsUnlockedStatus
    {
        public Card card;
        public bool isUnlocked;
    }

    public static CardsUnlocked GetInstance()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<CardsUnlocked>("CardsUnlocked");
            _instance.AddAllCards();
            _instance.SetDefaultUnlocked();
        }
        return _instance;
    }
    public void AddAllCards()
    {
        cardsUnlocked = new CardsUnlockedStatus[allCards.GetCards().Length];
        for (int i = 0; i < allCards.GetCards().Length; i++)
        {
            cardsUnlocked[i].card = allCards.GetCards()[i];
            cardsUnlocked[i].isUnlocked = false;
        }
        Debug.Log(cardsUnlocked.Length);
    }
    public void SetDefaultUnlocked()
    {
        for (int i = 0; i < defaultUnlocked.GetCards().Length; i++)
        {
            for (int j = 0; j < allCards.GetCards().Length; j++)
            {
                if(allCards.GetCards()[j] == defaultUnlocked.GetCards()[i])
                {
                    cardsUnlocked[j].isUnlocked = true;
                }
            }
        }
    }
    public List<Card> GetUnlockedCards()
    {
        List<Card> newCardList = new List<Card>();
        for (int i = 0; i < cardsUnlocked.Length; ++i)
        {
            if (cardsUnlocked[i].isUnlocked)
            {
                newCardList.Add(cardsUnlocked[i].card);
            }
        }
        return newCardList;
    }
    public List<Card> GetCardsByRarity(CardRarity rarity)
    {
        List<Card> newCardList = new List<Card>();
        for (int i = 0; i < cardsUnlocked.Length; ++i)
        {
            if (cardsUnlocked[i].isUnlocked && cardsUnlocked[i].card.rarity == rarity)
            {
                newCardList.Add(cardsUnlocked[i].card);
            }
        }
        return newCardList;
    }
}
