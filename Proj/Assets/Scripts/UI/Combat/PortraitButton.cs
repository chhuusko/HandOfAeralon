using System;
using UnityEngine;
using UnityEngine.UI;

public class PortraitButton : MonoBehaviour
{
    public event Action<Character> OnClickPortraitButton;

    public Character Character { get; set; }
    
    [SerializeField] private Button _button;
    public Button Button => _button;

    public void OnClick()
    {
        OnClickPortraitButton?.Invoke(Character);
        Selector._instance.SelectCharacterFromUI(Character);
    }
}