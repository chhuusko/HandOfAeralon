
using System.Collections.Generic;
using UnityEngine;
public enum CardType
{
    Instant,
    Target,
}
public enum TargetCondition
{
    None,
    Ally,
    Enemy,
    NotActive
}
public enum CardTag
{
    Ephemeral,
    Exhaust
}
public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
}
public class Card : ScriptableObject
{

    [Header("Info")]
    public CardType type;
    public Faction targetFaction;
    public CardRarity rarity;
    
    public string title;
    [TextArea(5, 10)] public string description;
    [SerializeField] private int cost;
    public Sprite icon;
    public Sprite CardTemplate;
    public List<InfoPanel> info;
    public List<CardTag> tags;
    public List<TargetCondition> targetConditions;

    private List<Color> rarityColors = new List<Color>()
    {
        new Color(0.80f, 0.54f, 0.49f),
        new Color(1f,1f,1f),
        new Color(1.00f, 0.66f, 0.14f)
    };

    private int tempCost;
    private bool isTempCost;
    public int GetCost()
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
        //card play anywhere
    }
    public virtual void PlayCardOnTarget(Character character)
    {
        //card play on target
        PlayCard();
    }
    public virtual void CardSelect(Card selectedCard)
    {

    }
    public virtual void AfterCardPlay()
    {
        ResetCost();
        CardHandManager.GetInstance().CardUsed(this);
    }
    public void ResetCost()
    {
        tempCost = cost;
        isTempCost = false;
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
        if (!isTempCost)
        {
            tempCost = cost;
        }
        isTempCost = true;
        tempCost += changeInCost;
        Debug.Log(tempCost + " tempcost " + changeInCost + " changeincost");
        if (tempCost < 0) { tempCost = 0; }
    }
    public bool GetIsTemp() 
    {
        return isTempCost;
    }
    public Color GetRarityColor(int rarity)
    {
        return rarityColors[rarity];
    }
    public virtual int GetDamage(Character character)
    {
        return 0;
    }
    public virtual void ShowDamagePreview(Character character)
    {
        if (character == null) return;
        character.PreviewHealthChange(-GetDamage(character));
    }
    public virtual void ShowDamagePreview()
    {

    }
    public virtual string GetDescription()
    {
        return description;
    }
}

