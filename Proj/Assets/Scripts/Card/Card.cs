
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
    Etherial,
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

    protected int damage;

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
        //n�r den spelas
    }
    public virtual void PlayCardOnTarget(Character character)
    {
        //n�r den spelas p� en target
        PlayCard();
    }
    public virtual void CardSelect(Card selectedCard)
    {

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
        damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
        return damage;
    }
    public virtual void ShowDamagePreview(Character character)
    {
        if (character == null) return;
        character.PreviewHealthChange(GetDamage(character));
    }
    public virtual void ShowDamagePreview()
    {

    }
}

