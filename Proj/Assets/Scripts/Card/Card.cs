
using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Item/Card Data", order = 1)]
public class Card : ScriptableObject
{
    public enum CardType
    {
        Instant,
        Select,
    }

    [Header("Info")]
    public CardType type;
    public string title;
    public string description;
    public int cost;
    public Sprite icon;
    
    public void PlayCard()
    {
        //när den spelas


    }
}

