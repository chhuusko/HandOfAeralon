using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual combat log entry, to be added to the combat log.
/// </summary>
public abstract class CombatLogEntry : MonoBehaviour
{
    [SerializeField] protected Image _image;
    [SerializeField] protected TMP_Text _text;
    
    public abstract void Initialize(CombatLogData data);
}
