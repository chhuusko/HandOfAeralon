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
}


public static class DebugLog
{
    private static DEBUGSettings _settings;

    private static void LoadSettings()
    {
        if (_settings == null)
            _settings = AssetDatabase.LoadAssetAtPath<DEBUGSettings>("Assets/ScriptableObject/DEBUG/DEBUGSettings.asset");
    }
    public static void CJLog(string message)
    {
        LoadSettings();
        if (_settings && _settings.CJLog)
            Debug.Log("CJ Log: " + message);
    }
    public static void MGLog(string message)
    {
        LoadSettings();
        if (_settings && _settings.MGLog)

            Debug.Log("MG Log: " + message);
    }
    public static void JLWLog(string message)
    {
        LoadSettings();
        if (_settings && _settings.JLWLog)

            Debug.Log("JLW Log: " + message);
    }
    public static void AlexLog(string message)
    {
        LoadSettings();
        if (_settings && _settings.AlexLog)

            Debug.Log("Alex Log: " + message);
    }
    public static void JoppaLog(string message)
    {
        LoadSettings();
        if (_settings && _settings.JoppaLog)

            Debug.Log("Joppa Log: " + message);
    }
};