using UnityEngine;
[CreateAssetMenu(fileName = "CardList", menuName = "Item/Card List", order = 1)]

public class CardList : ScriptableObject
{
   
    [Header("Info")]
    [SerializeField] private Card[] _cards;
    public Card GetRandomCard()
    {
        return _cards[Random.Range(0, _cards.Length)];
    }
    public Card[] GetCards()
    {
        return _cards;
    }
    
}
