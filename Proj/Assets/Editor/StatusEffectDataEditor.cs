#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(StatusEffectData), true)]
public class StatusEffectDataEditor : Editor
{
    private Type[] availableTypes;
    private string[] typeNames;

    private void OnEnable()
    {
        var baseType = typeof(StatusEffect);
        availableTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t != baseType)
            .OrderBy(t => t.Name)
            .ToArray();
        typeNames = availableTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
       serializedObject.Update();
       
       SerializedProperty iterator = serializedObject.GetIterator();
       bool enterChildren = true;

       while (iterator.NextVisible(enterChildren))
       {
           enterChildren = false;

           if (iterator.name == "m_Script" || iterator.name == "statusEffectTypeName")
           {
               continue;
           }
           
           EditorGUILayout.PropertyField(iterator, true);
       }
       
       EditorGUILayout.Space();
       EditorGUILayout.LabelField("Status Effect Type", EditorStyles.boldLabel);
       
       var statusEffectTypeNameProp = serializedObject.FindProperty("_typeName");

       if (statusEffectTypeNameProp != null)
       {
           int currentIndex = GetCurrentTypeIndex(statusEffectTypeNameProp.stringValue);
           int newIndex = EditorGUILayout.Popup("Effect Class", currentIndex, typeNames);

           if (newIndex != currentIndex && newIndex >= 0)
           {
               statusEffectTypeNameProp.stringValue = availableTypes[newIndex].AssemblyQualifiedName;
           }
           
           if (currentIndex >= 0)
           {
               EditorGUILayout.HelpBox($"Type: {availableTypes[currentIndex].FullName}", MessageType.Info);
           }
           else
           {
               EditorGUILayout.HelpBox("No status effect type assigned!", MessageType.Warning);
           }
       }
       else
       {
           EditorGUILayout.HelpBox("StatusEffectTypeName field not found!", MessageType.Error);
       }
       
       serializedObject.ApplyModifiedProperties();
    }

    private int GetCurrentTypeIndex(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return -1;
        }
        
        var currentType = Type.GetType(typeName);
        if (currentType == null)
        {
            return -1;
        }
        
        return Array.IndexOf(availableTypes, currentType);
    }
}
#endif