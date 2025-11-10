using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Barbarian, Wizard, Rogue, Bard }

[CreateAssetMenu(fileName = "ClassData", menuName = "Character/ClassData")]
public class ClassData : ScriptableObject
{
    public CharacterClass characterClass;
    
    [Header("Base Stats")] 
    public int minHealthPoints;
    public int maxHealthPoints;
    public int minInitiative;
    public int maxInitiative;
    
    [Header("Abilities")]
    public List<Ability> abilities;
}
