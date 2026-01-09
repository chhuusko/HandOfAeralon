using System;
using System.Collections.Generic;
using System.Linq;
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
    private static Dictionary<string, StatusEffectData> _dataPerName = new();
    public static Dictionary<string, StatusEffectData> DataPerName => _dataPerName;

    private void Initialize()
    {
        _lookup = new Dictionary<Type, StatusEffectData>();
        foreach (var entry in _entries)
        {
            if (entry == null)
            {
                DebugLog.JoppaLog("No entry");
                continue;
            }
            
            Type type = entry.GetEffectType();
            if (type != null)
            {
                _lookup[type] = entry;
                _dataPerName.TryAdd(type.Name, entry);
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

    private IReadOnlyList<StatusEffectData> GetAllTraits()
    {
        return _entries.Where(e => e.Type == StatusEffectType.Trait).ToList();
    }

    public IReadOnlyList<StatusEffectData> GetAllGlobalTraitsOfType(bool isPositive)
    {
        List<StatusEffectData> traitsOfType = new();

        foreach (StatusEffectData trait in GetAllTraits())
        {
            if (trait.Class == CharacterClass.None && trait.IsPositive == isPositive)
            {
                traitsOfType.Add(trait);
            }
        }

        return traitsOfType;
    }

    public IReadOnlyList<StatusEffectData> GetAllClassTraits(CharacterData character)
    {
        List<StatusEffectData> traits = new();
        
        foreach (StatusEffectData trait in GetAllTraits())
        {
            if (trait.Class == character.CharacterClass)
            {
                traits.Add(trait);
            }
        }
        
        return traits;
    }
}