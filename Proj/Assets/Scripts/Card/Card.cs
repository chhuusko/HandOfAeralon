
using System.Collections.Generic;
using UnityEngine;
public enum CardType
{
    Instant,
    Target,
}
public enum CardTag
{
    Etherial,
    Exhaust
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
    private bool isTempCost;
    private void Awake()
    {
        
    }
    public int Getcost()
    {
        if (isTempCost)
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
    public virtual void PlayCardOnTarget(Character character)
    {
        //när den spelas på en target
        PlayCard();
    }
    public virtual void AfterCardPlay()
    {
        tempCost = cost;
        isTempCost = false;
        CardHandManager.GetInstance().CardUsed(this);
    }
    public T Clone<T>() where T : ScriptableObject
    {
        T copy = Instantiate(this) as T;
        return copy;
    }
    public void TempSetCost(int newTempCost)
    {
        isTempCost = true;
        tempCost = newTempCost;
        if (tempCost < 0) { tempCost = 0; }
    }
    public void TempModifyCost(int changeInCost)
    {
        isTempCost = true;
        tempCost += changeInCost;
        if (tempCost < 0) { tempCost = 0; }
    }
}

