using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardPackUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int _cost;
    [SerializeField] int CardAmount;
    List<Card> cardInPack = new List<Card>();
    [SerializeField] CardPackViewUIShop cardPackView;
    private void Awake()
    {
        RandomizeCards();
    }
    private void RandomizeCards()
    {
        List<Card> temp = CardsUnlocked.GetInstance().GetUnlockedCards();

        for (int i = 0; i < CardAmount; i++)
        {
            cardInPack.Add(temp[Random.Range(0, temp.Count)]);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Shop.CanAfford(_cost))
        {
           cardPackView.UpdateCards(cardInPack);
        }
        
    }
}
