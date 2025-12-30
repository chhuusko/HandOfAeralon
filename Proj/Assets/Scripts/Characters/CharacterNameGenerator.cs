using System.Collections.Generic;
using UnityEngine;

public class CharacterNameGenerator
{
    private static Dictionary<ClassData, List<string>> _availableNamesPerClass = new();
    private static bool _isInitialized;

    private static void Initialize()
    {
        _availableNamesPerClass.Clear();
        
        var classDatabase = Resources.Load<ClassDatabase>("Characters/ClassDatabase");

        if (!classDatabase)
        {
            return;
        }
        
        foreach (var classData in classDatabase.Classes)
        {
            _availableNamesPerClass[classData] = new List<string>(classData.availableNames);
        }

        _isInitialized = true;
    }
    
    public static string GenerateName(ClassData classData)
    {
        if (!classData)
        {
            return null;
        }

        if (!_isInitialized)
        {
            Initialize();
        }
        
        if (!_availableNamesPerClass.ContainsKey(classData) || _availableNamesPerClass[classData].Count == 0)
        {
            // Resets all names for this class if none are left.
            _availableNamesPerClass[classData] = new List<string>(classData.availableNames);
        }
        
        var names = _availableNamesPerClass[classData];
        if (names.Count == 0)
        {
            Debug.LogError("No names available");
            return "NoName";
        }
        
        int index = UnityEngine.Random.Range(0, _availableNamesPerClass[classData].Count);
        string name = names[index];
        _availableNamesPerClass[classData].RemoveAt(index);
        
        return name;
    }
}
