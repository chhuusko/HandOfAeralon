using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectDataRegistry", menuName = "StatusEffects/StatusEffectDataRegistry")]
public class StatusEffectDataRegistry : ScriptableObject
{
    // [Serializable]
    // private struct Entry
    // {
    //     public StatusEffectData Data;
    // }
    
    [SerializeField] private StatusEffectData[] entries;
    private static Dictionary<Type, StatusEffectData> _lookup;

    public void Initialize()
    {
        _lookup = new Dictionary<Type, StatusEffectData>();
        foreach (var entry in entries)
        {
            Type type = Type.GetType(entry.Name);
            if (type != null)
            {
                _lookup[type] = entry;
            }
            else
            {
                Debug.LogError($"Unknown status effect type: {entry.Name}");
            }
        }
    }

    public static StatusEffectData GetDataForType(Type type)
    {
        if (_lookup == null)
        {
            return null;
        }
        
        _lookup.TryGetValue(type, out var data);
        return data;
    }
}
