using System;
using UnityEngine;

public class PortraitButton : MonoBehaviour
{
    public event Action<PortraitButton> OnClickPortraitButton;

    public CharacterData Character { get; set; }

    public void OnClick()
    {
        OnClickPortraitButton?.Invoke(this);
        Selector._instance.SetSelectedCharacter(CombatManager._instance.GetCharacterDataDict()[Character]);
    }
}