
using System.Collections.Generic;
using UnityEngine;
public enum CardType
{
    Instant,
    Target,
}
public enum CardTag
{
    Etherial
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
    [SerializeField] private int cost;
    public Sprite icon;
    public Sprite CardTemplate;
    public List<InfoPanel> info;
    public List<CardTag> tags;

    private int tempCost;

    public int Getcost()
    {
        if (tempCost != 0)
        {
            return tempCost;
        }
        else
        {
            return cost;
        }
    }
    public virtual void PlayCard()
    {
        //när den spelas
    }
    public virtual void AfterCardPlay()
    {
        tempCost = 0;
    }
    public T Clone<T>() where T : ScriptableObject
    {
        // Create a new instance in memory (not saved as an asset)
        T copy = Instantiate(this) as T;
        return copy;
    }
    public void TempSetCost(int newTempCost)
    {
        tempCost = newTempCost;
    }
    public void TempModifyCost(int changeInCost)
    {
        if (tempCost == 0)
        {
            tempCost = cost;
        }
        
        tempCost += changeInCost;
        if (tempCost < 0) { tempCost = 0; }
    }
}

