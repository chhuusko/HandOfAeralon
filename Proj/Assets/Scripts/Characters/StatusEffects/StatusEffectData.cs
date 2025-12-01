using UnityEditor;
using UnityEngine;

public enum StatusEffectType { Buff, Debuff, ArenaEffect, Trait, CrowdControl }

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffects/StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public StatusEffectType Type;
    public bool IsPermanent;
    public MonoScript Script;

    /// <summary>
    /// Factory method for generating a status effect from the data.
    /// Used when we don't know beforehand which status effect to create.
    /// </summary>
    /// <param name="duration">The amount of turns the status effect lasts.</param>
    /// <returns></returns>
    public StatusEffect CreateInstance(int duration = 3)
    {
        var type = Script.GetClass();

        if (!typeof(StatusEffect).IsAssignableFrom(type))
        {
            return null;
        }
        
        return (StatusEffect)System.Activator.CreateInstance(type, duration);
    }
}
