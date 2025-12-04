
using System.Collections.Generic;
using UnityEngine;
public enum CardType
{
    Instant,
    Target,
}
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
}
public class Card : ScriptableObject
{
    
    

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
    public T Clone<T>() where T : ScriptableObject
    {
        // Create a new instance in memory (not saved as an asset)
        T copy = Instantiate(this) as T;
        return copy;
    }
}

