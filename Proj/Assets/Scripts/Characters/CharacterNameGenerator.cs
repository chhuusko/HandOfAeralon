using System.Collections.Generic;
using UnityEngine;

public class CharacterNameGenerator
{
    private static Dictionary<ClassData, List<string>> _allNamesPerClass = new();

    private static void Initialize()
    {
        _allNamesPerClass.Clear();
        
        var classDatabase = Resources.Load<ClassDatabase>("Characters/ClassDatabase");

        if (!classDatabase)
        {
            return;
        }
        
        foreach (var classData in classDatabase.Classes)
        {
            _allNamesPerClass[classData] = new List<string>(classData.availableNames);
        }
    }

    public static string GenerateName(ClassData classData, HashSet<string> reservedNames)
    {
        if (!classData)
        {
            Debug.LogWarning("No class data found");
            return null;
        }

        if (!_allNamesPerClass.ContainsKey(classData))
        {
            Initialize();
        }
        
        var allNames = _allNamesPerClass[classData];
        
        var availableNames = allNames.FindAll(
            name => !reservedNames.Contains(name));

        if (availableNames.Count == 0)
        {
            Debug.LogWarning("No available names found");
            availableNames = new List<string>(allNames);
        }
        
        int index = UnityEngine.Random.Range(0, availableNames.Count);
        return availableNames[index];
    }
}
