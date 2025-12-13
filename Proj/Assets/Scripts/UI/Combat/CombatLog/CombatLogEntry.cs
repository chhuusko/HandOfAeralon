using System;
using UnityEngine;

public class CombatLogEntry : MonoBehaviour
{
    public Ability Ability { get; private set; }
    public CharacterData Source { get; private set; }
    public CharacterData Target { get; private set; }

    public CombatLogEntry(Ability ability, CharacterData source, CharacterData target)
    {
        Ability = ability;
        Source = source;
        Target = target;
    }
}
