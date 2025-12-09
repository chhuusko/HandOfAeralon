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

    [SerializeField, HideInInspector] protected string _typeName;
    
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
        var type = GetEffectType();

        if (type == null || !typeof(StatusEffect).IsAssignableFrom(type))
        {
            return null;
        }
        
        return (StatusEffect)System.Activator.CreateInstance(type, duration);
    }

    /// <summary>
    /// Factory method for generating a status effect from its data.
    /// Used when the status effect to create isn't known beforehand.
    /// </summary>
    /// <returns>The created status effect.</returns>
    public StatusEffect CreateInstance()
    {
        var type = GetEffectType();

        if (type == null || !typeof(StatusEffect).IsAssignableFrom(type))
        {
            return null;
        }
        
        return (StatusEffect)System.Activator.CreateInstance(type);
    }

    /// <summary>
    /// Get the type of status effect associated with this data.
    /// </summary>
    /// <returns>Type of status effect.</returns>
    public System.Type GetEffectType()
    {
        return System.Type.GetType(_typeName);
    }
}
