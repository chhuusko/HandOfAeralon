using UnityEditor;
using UnityEngine;

public enum StatusEffectType { Buff, Debuff, ArenaEffect, Trait }

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    [Header("General Info")]
    public string Name;
    public Sprite Icon;
    public string Description;
    public StatusEffectType Type;
    public bool IsPermanent;
    public MonoScript Script;
    
    [Header("Traits")]
    public CharacterClass Class;
    public bool IsPositive;

    /// <summary>
    /// Factory method for generating a status effect from the data.
    /// Used when we don't know beforehand which status effect to create.
    /// </summary>
    /// <param name="duration">The amount of turns the status effect lasts.</param>
    /// <returns></returns>
    public StatusEffect CreateInstance(int duration)
    {
        var type = Script.GetClass();

        if (!typeof(StatusEffect).IsAssignableFrom(type))
        {
            return null;
        }
        
        return (StatusEffect)System.Activator.CreateInstance(type, duration);
    }

    public StatusEffect CreateInstance()
    {
        var type = Script.GetClass();

        if (!typeof(StatusEffect).IsAssignableFrom(type))
        {
            return null;
        }
        
        return (StatusEffect)System.Activator.CreateInstance(type);
    }
}
