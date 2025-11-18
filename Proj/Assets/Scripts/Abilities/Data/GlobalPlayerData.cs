using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerData : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static GlobalPlayerData _instance;
    private List<Character> heroList;
    private List<Card> cardsList;
    private void Awake()
    {
        _instance = this;
    }
    public GlobalPlayerData GetInstance()
    {
        return _instance;
    }
    public List<Character> GetHeroList()
    {
        return heroList;
    }
    public List<Card> GetCardList()
    {
        return cardsList;
    }
    public void SetHeroList(List<Character> newHeroList)
    {
        heroList = newHeroList;
    }
    public void SetCardList(List<Character> newHeroList)
    {
        heroList = newHeroList;
    }
    public void AddCharacter(Character hero)
    {
        heroList.Add(hero);
    }
    public void RemoveCharacter(Character hero)
    {
        heroList.Remove(hero); 
    }
}
