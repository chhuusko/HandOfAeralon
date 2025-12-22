using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CombatLogEntry : MonoBehaviour
{
    [SerializeField] protected Image _image;
    [SerializeField] protected TMP_Text _text;
    
    public abstract void Initialize(CombatLogData data);
}
