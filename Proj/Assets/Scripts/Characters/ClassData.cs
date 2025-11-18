using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Barbarian, Wizard, Rogue, Bard, None }

[CreateAssetMenu(fileName = "ClassData", menuName = "Character/ClassData")]
public class ClassData : ScriptableObject
{
    public CharacterClass characterClass;
    
    [Header("Base Stats")] 
    public int minHealthPoints;
    public int maxHealthPoints;
    public int minSpeed;
    public int maxSpeed;
    public int minMovementPoints;
    public int maxMovementPoints;
    public int minDamage;
    public int maxDamage;
    
    [Header("Abilities")]
    public List<Ability> abilities;

    [Header("Misc")]
    public Sprite classImage;
}
