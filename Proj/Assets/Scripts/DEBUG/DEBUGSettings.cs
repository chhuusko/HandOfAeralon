using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DEBUGSettings", menuName = "Scriptable Objects/DEBUGSettings")]
public class DEBUGSettings : ScriptableObject
{
    public bool CJLog = true;
    public bool MGLog = true;
    public bool JLWLog = true;
    public bool AlexLog = true;
    public bool JoppaLog = true;
    public bool CombatLog = true;
}


public static class DebugLog
{
    private static DEBUGSettings _settings;

    private static void LoadSettings()
    {
        #if UNITY_EDITOR
        if (_settings == null)
            _settings = Resources.Load<DEBUGSettings>("ScriptableObjects/DEBUGSettings");
        #endif
    }

    // Standard Logs
    public static void CJLog(string message)            { LoadSettings(); Log(_settings.CJLog, "CJ", message); }
    public static void MGLog(string message)            { LoadSettings(); Log(_settings.MGLog, "MG", message); }
    public static void JLWLog(string message)           { LoadSettings(); Log(_settings.JLWLog, "JLW", message); }
    public static void AlexLog(string message)          { LoadSettings(); Log(_settings.AlexLog, "Alex", message); }
    public static void JoppaLog(string message)         { LoadSettings(); Log(_settings.JoppaLog, "Joppa", message); }
    public static void CombatLog(string message)        { LoadSettings(); Log(_settings.CombatLog, "Combat", message); }

    // Error Logs
    public static void CJLogError(string message)       { LoadSettings(); LogError(_settings.CJLog, "CJ", message); }
    public static void MGLogError(string message)       { LoadSettings(); LogError(_settings.MGLog, "MG", message); }
    public static void JLWLogError(string message)      { LoadSettings(); LogError(_settings.JLWLog, "JLW", message); }
    public static void AlexLogError(string message)     { LoadSettings(); LogError(_settings.AlexLog, "Alex", message); }
    public static void JoppaLogError(string message)    { LoadSettings(); LogError(_settings.JoppaLog, "Joppa", message); }
    public static void CombatLogError(string message)   { LoadSettings(); LogError(_settings.CombatLog, "Combat", message); }

    // Warning Logs
    public static void CJLogWarning(string message)     { LoadSettings(); LogWarning(_settings.CJLog, "CJ", message); }
    public static void MGLogWarning(string message)     { LoadSettings(); LogWarning(_settings.MGLog, "MG", message); }
    public static void JLWLogWarning(string message)    { LoadSettings(); LogWarning(_settings.JLWLog, "JLW", message); }
    public static void AlexLogWarning(string message)   { LoadSettings(); LogWarning(_settings.AlexLog, "Alex", message); }
    public static void JoppaLogWarning(string message)  { LoadSettings(); LogWarning(_settings.JoppaLog, "Joppa", message); }
    public static void CombatLogWarning(string message) { LoadSettings(); LogWarning(_settings.CombatLog, "Combat", message); }

    // Internals
    private static void Log(bool enabled, string name, string message)
    {
        LoadSettings();                  // Ensure settings are loaded
        if (enabled) Debug.Log($"{name} Log: {message}");
    }

    private static void LogError(bool enabled, string name, string message)
    {
        LoadSettings();
        if (enabled) Debug.LogError($"{name} Error: {message}");
    }

    private static void LogWarning(bool enabled, string name, string message)
    {
        LoadSettings();
        if (enabled) Debug.LogWarning($"{name} Warning: {message}");
    }
};