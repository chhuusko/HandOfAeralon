using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectDataRegistry", menuName = "StatusEffects/StatusEffectDataRegistry")]
public class StatusEffectDataRegistry : ScriptableObject
{
    private static StatusEffectDataRegistry _instance;
    public static StatusEffectDataRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<StatusEffectDataRegistry>("Characters/StatusEffects/StatusEffectDataRegistry");
                _instance?.Initialize();
            }

            return _instance;
        }
    }
    
    [SerializeField] private StatusEffectData[] _entries;
    private static Dictionary<Type, StatusEffectData> _lookup;

    public void Initialize()
    {
        _lookup = new Dictionary<Type, StatusEffectData>();
        foreach (var entry in _entries)
        {
            Type type = entry.Script.GetClass();
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

    public IReadOnlyList<StatusEffectData> GetAllData()
    {
        return _lookup.Values.ToList();
    }

    public IReadOnlyList<TraitData> GetAllTraits()
    {
        return _entries.OfType<TraitData>().ToList();
    }

    public IReadOnlyList<TraitData> GetAllTraitsOfType(bool isPositive)
    {
        List<TraitData> traitsOfType = new();

        foreach (TraitData trait in GetAllTraits())
        {
            if (trait.IsPositive == isPositive)
            {
                traitsOfType.Add(trait);
            }
        }

        return traitsOfType;
    }
}