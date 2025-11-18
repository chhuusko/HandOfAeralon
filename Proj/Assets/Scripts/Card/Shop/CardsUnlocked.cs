using System;
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
            Debug.Log("instantiatie");
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
            Debug.Log("added");
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
}
