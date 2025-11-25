using UnityEngine;

public class Poison : StatusEffect, IStackable
{
    public Poison(int duration) : base(duration)
    {
        Stacks = duration;
    }

    public int Stacks { get; private set; }
    
    public void AddStack(int amount = 1)
    {
        Stacks += amount;
    }

    public void RemoveStack(int amount = 1)
    {
        Stacks -= amount;
        if (Stacks <= 0)
        {
            Manager.RemoveStatusEffect(this);
        }
    }

    public override void OnTurnStart()
    {
        Character.TakeDamage(Stacks);
        RemoveStack();
    }
}
