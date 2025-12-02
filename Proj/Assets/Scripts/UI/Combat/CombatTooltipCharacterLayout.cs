using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CombatTooltipCharacterLayout : MonoBehaviour
{
    private enum CharacterStatKey
    {
        CurrentHealth,
        CurrentInitiative,
        CurrentDamage,
        CurrentMovementPoints,

        BaseHealth,
        BaseInitiative,
        BaseDamage,
        BaseMovementPoints
    };

    private List<string> _characterStatValues = new List<string>();
    [SerializeField] private TMP_Text _characterStatValueFieldTMP;

    private void OnEnable()
    {
        Selector._instance.OnCharacterSelected += UpdateTooltip;
    }

    public void InitializeCharacterStats()
    {
        _characterStatValues.Insert((int)CharacterStatKey.CurrentHealth, "10");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentInitiative, "10");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentDamage, "10");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentMovementPoints, "10\n");

        _characterStatValues.Insert((int)CharacterStatKey.BaseHealth, "10");
        _characterStatValues.Insert((int)CharacterStatKey.BaseInitiative, "10");
        _characterStatValues.Insert((int)CharacterStatKey.BaseDamage, "10");
        _characterStatValues.Insert((int)CharacterStatKey.BaseMovementPoints, "10");

        _characterStatValueFieldTMP.text = "";

        foreach (string value in _characterStatValues)
        {
            _characterStatValueFieldTMP.text += value + "\n";
        }
    }

    public void RebuildCharacterStatTooltip(Character character)
    {
        _characterStatValues.Insert((int)CharacterStatKey.CurrentHealth, $"{character.GetCurrentHealth()}");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentInitiative, $"{character.GetInitiative()}");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentDamage, $"{character.GetDamage()}");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentMovementPoints, $"{character.GetMovementPoints()}" + "\n");

        _characterStatValues.Insert((int)CharacterStatKey.BaseHealth, $"{character.GetMaxHealth()}");
        _characterStatValues.Insert((int)CharacterStatKey.BaseInitiative, $"{character.GetBaseInitiative()}");
        _characterStatValues.Insert((int)CharacterStatKey.BaseDamage, $"{character.GetBaseDamage()}");
        _characterStatValues.Insert((int)CharacterStatKey.BaseMovementPoints, $"{character.GetBaseMovementPoints()}" + "\n");
    }

    private void UpdateTooltip(Character character)
    {
        RebuildCharacterStatTooltip(character);
    }

    public void ShowCharacterTooltip()
    {

    }
}
