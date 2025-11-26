using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static GlobalPlayerData _instance;
    private List<CharacterData> heroList;
    private List<Card> cardsList;
    private void Awake()
    {
        
        _instance = this;
    }
    public GlobalPlayerData GetInstance()
    {
        return _instance;
    }
    public List<CharacterData> GetHeroList()
    {
        return heroList;
    }
    public List<Card> GetCardList()
    {
        return cardsList;
    }
    public void SetHeroList(List<CharacterData> newHeroList)
    {
        heroList = newHeroList;
    }
    public void SetCardList(List<CharacterData> newHeroList)
    {
        heroList = newHeroList;
    }
    public void AddCharacter(CharacterData hero)
    {
        heroList.Add(hero);
    }
    public void RemoveCharacter(CharacterData hero)
    {
        heroList.Remove(hero); 
    }
}
