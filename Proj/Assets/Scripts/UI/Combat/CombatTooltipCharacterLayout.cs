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

    public void BindEventEventOnTakeDamage(Character character)
    {
        character.OnTakeDamage += UpdateTooltip;

    }
    public void UnBindEventEventOnTakeDamage(Character character)
    {
        character.OnTakeDamage -= UpdateTooltip;
    }
    public void InitializeCharacterStats()
    {
        _characterStatValues.Insert((int)CharacterStatKey.CurrentHealth, "");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentInitiative, "");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentDamage, "");
        _characterStatValues.Insert((int)CharacterStatKey.CurrentMovementPoints, "\n");

        _characterStatValues.Insert((int)CharacterStatKey.BaseHealth, "");
        _characterStatValues.Insert((int)CharacterStatKey.BaseInitiative, "");
        _characterStatValues.Insert((int)CharacterStatKey.BaseDamage, "");
        _characterStatValues.Insert((int)CharacterStatKey.BaseMovementPoints, "");

        _characterStatValueFieldTMP.text = "";

        foreach (string value in _characterStatValues)
        {
            _characterStatValueFieldTMP.text += value + "\n";
        }


    }

    public void RebuildCharacterStatTooltip(Character character)
    {

        _characterStatValues[(int)CharacterStatKey.CurrentHealth]          = $"{character.GetCurrentHealth()}";
        _characterStatValues[(int)CharacterStatKey.CurrentInitiative]      = $"{character.GetInitiative()}";
        _characterStatValues[(int)CharacterStatKey.CurrentDamage]          = $"{character.GetDamage()}";
        _characterStatValues[(int)CharacterStatKey.CurrentMovementPoints]  = $"{character.GetMovementPoints()}\n";            

        _characterStatValues[(int)CharacterStatKey.BaseHealth]             = $"{character.GetMaxHealth()}";
        _characterStatValues[(int)CharacterStatKey.BaseInitiative]         = $"{character.GetBaseInitiative()}";
        _characterStatValues[(int)CharacterStatKey.BaseDamage]             = $"{character.GetBaseDamage()}";
        _characterStatValues[(int)CharacterStatKey.BaseMovementPoints]     = $"{character.GetBaseMovementPoints()}";

        string stats = "";
        foreach (string value in _characterStatValues)
        {
            stats += value + "\n";
        }
        _characterStatValueFieldTMP.text = stats;
    }

    private void UpdateTooltip(Character character)
    {
        RebuildCharacterStatTooltip(character);
    }
    private void UpdateTooltip(int health, GameObject character)
    {
        RebuildCharacterStatTooltip(character.GetComponent<Character>());
    }
    private void UpdateTooltipOnDamage(int damage, Character character)
    {
        RebuildCharacterStatTooltip(character);
    }
    public void ShowCharacterTooltip()
    {

    }
}
