using System;
using UnityEngine;
using UnityEngine.UI;

public class PortraitButton : MonoBehaviour
{
    public event Action<PortraitButton> OnClickPortraitButton;

    public CharacterData Character { get; set; }
    
    [SerializeField] private Button _button;
    public Button Button => _button;

    public void OnClick()
    {
        // Don't update UI for selecting enemy characters.
        if (Character.Faction == Faction.Enemy)
        {
            return;
        }
        OnClickPortraitButton?.Invoke(this);
        Selector._instance.SelectCharacterFromUI(CombatManager._instance.GetCharacterDataDict()[Character]);
    }
}