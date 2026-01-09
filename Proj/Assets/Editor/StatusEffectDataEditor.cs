#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(StatusEffectData), true)]
public class StatusEffectDataEditor : Editor
{
    private Type[] availableTypes;
    private string[] typeDisplayNames;

    private void OnEnable()
    {
        // Cache available types
        var baseType = typeof(StatusEffect);
        availableTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t != baseType)
            .OrderBy(t => t.Name)
            .ToArray();
        
        typeDisplayNames = availableTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw all properties manually, skipping statusEffectTypeName
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            
            // Skip the script field and our custom field
            if (iterator.name == "m_Script" || iterator.name == "statusEffectTypeName")
                continue;
            
            EditorGUILayout.PropertyField(iterator, true);
        }

        // Now draw our custom Effect Class dropdown
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status Effect Type", EditorStyles.boldLabel);
        
        var statusEffectTypeNameProp = serializedObject.FindProperty("_typeName");
        
        if (statusEffectTypeNameProp != null)
        {
            int currentIndex = GetCurrentTypeIndex(statusEffectTypeNameProp.stringValue);
            int newIndex = EditorGUILayout.Popup("Effect Class", currentIndex, typeDisplayNames);

            if (newIndex != currentIndex && newIndex >= 0)
            {
                statusEffectTypeNameProp.stringValue = availableTypes[newIndex].AssemblyQualifiedName;
            }

            // Show current type info
            if (currentIndex >= 0)
            {
                EditorGUILayout.HelpBox($"Type: {availableTypes[currentIndex].FullName}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("No status effect type assigned", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("statusEffectTypeName field not found", MessageType.Error);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private int GetCurrentTypeIndex(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
            return -1;

        var currentType = Type.GetType(typeName);
        if (currentType == null)
            return -1;

        return Array.IndexOf(availableTypes, currentType);
    }
}
#endif