using UnityEngine;
[CreateAssetMenu(fileName = "CardList", menuName = "Item/Card List", order = 1)]

public class CardList : ScriptableObject
{
   
    [Header("Info")]
    [SerializeField] private Card[] cards;
    public Card GetRandomCard()
    {
        return cards[Random.Range(0, cards.Length)];
    }
    
}
