using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Barbarian, Sorceress, Rogue, Bard, None }

[CreateAssetMenu(fileName = "ClassData", menuName = "Character/ClassData")]
public class ClassData : ScriptableObject
{
    public CharacterClass characterClass;
    
    [Header("Base Stats")] 
    public int minHealthPoints;
    public int maxHealthPoints;
    public int minDamage;
    public int maxDamage;
    public int minInitiative;
    public int maxInitiative;
    public int minMovementPoints;
    public int maxMovementPoints;
    
    [Header("Abilities")]
    public List<Ability> abilities;

    [Header("Misc")]
    public Sprite classImage;
}
