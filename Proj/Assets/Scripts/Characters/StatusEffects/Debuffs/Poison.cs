using UnityEngine;

public class Poison : StatusEffect, IStackable
{
    public Poison(int duration) : base(duration)
    {
        Stacks = duration;
    }

    public int Stacks { get; }
    
    public void AddStack(int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveStack(int amount = 1)
    {
        throw new System.NotImplementedException();
    }
}
