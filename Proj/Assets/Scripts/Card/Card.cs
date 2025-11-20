
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Item/Card Data", order = 1)]
public class Card : ScriptableObject
{
    public enum CardType
    {
        Instant,
        Select,
    }
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
    }

    [Header("Info")]
    public CardType type;
    public Rarity rarity;
    public string title;
    public string description;
    public int cost;
    public Sprite icon;
    public Sprite CardTemplate;
    public List<InfoPanel> info;
    public virtual void PlayCard()
    {
        //när den spelas
    }
    
}

