using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombatTurnOrder
{
    private List<Character> _characters;
    private PriorityQueue<Character> _characterTurnQueue;

    public CombatTurnOrder()
    {
        _characters         = new List<Character>();
        _characterTurnQueue = new PriorityQueue<Character>();
    }


    public void InitializeTurnOrder()
    {
        foreach(Character c in GlobalGameManager.GetInstance().GetGameData().heroList)
        {
            //_characterTurnQueue.Enqueue(c, c.GetInitiative());
        }
    }

    public void RebuildTurnOrder()
    {

    }

    public void GetNextCharacterInTurn()
    {
        //_characterTurnQueue.de
    }

}
