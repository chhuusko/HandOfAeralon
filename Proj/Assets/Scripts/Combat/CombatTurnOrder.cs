using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


// NOTE (Calle): PriorityQueue is a MIN HEAP so small values are prioritized, therefor initiative has to be negated when 
//               inserted.

public class CombatTurnOrder
{
    [SerializeField] private List<Character> _characters;
    [SerializeField] private PriorityQueue<Character> _characterTurnQueue;

    public CombatTurnOrder()
    {
        _characters         = new List<Character>();
        _characterTurnQueue = new PriorityQueue<Character>();
    }


    public void InitializeTurnOrder()
    {
        _characters = CombatGrid._instance.GetAllCharacterScripts();
        
        foreach (Character c in _characters)
        {
            _characterTurnQueue.Enqueue(c, -c.GetInitiative());
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
