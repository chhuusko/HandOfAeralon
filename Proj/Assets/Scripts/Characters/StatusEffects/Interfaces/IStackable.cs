using UnityEngine;

public interface IStackable
{
    int Stacks { get; }
    void AddStack(int amount = 1);
    void RemoveStack(int amount = 1);
}
