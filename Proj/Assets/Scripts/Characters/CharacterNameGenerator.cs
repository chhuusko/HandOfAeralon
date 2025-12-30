using System.Collections.Generic;
using UnityEngine;

public class CharacterNameGenerator
{
    private static Dictionary<ClassData, List<string>> _names;
    private static bool _isInitialized;

    private static void Initialize()
    {
        _names = new();
        
        var classDatabase = Resources.Load<ClassDatabase>("Characters/ClassDatabase");

        if (!classDatabase)
        {
            return;
        }
        
        foreach (var classData in classDatabase.Classes)
        {
            _names[classData] = new List<string>(classData.availableNames);
        }
    }
    
    public static string GenerateName(Character character)
    {
        if (!_isInitialized)
        {
            Initialize();
        }
        
        var classData = character.GetClassData();
        int index = UnityEngine.Random.Range(0, _names[classData].Count);
        string name = _names[classData][index];
        _names[classData].RemoveAt(index);
        
        return name;
    }
}
