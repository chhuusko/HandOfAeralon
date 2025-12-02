using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    // Layout
    [SerializeField] private GameObject _layout;
    [SerializeField] private Image _characterIcon;
    [SerializeField] private TMP_Text _characterClassName;

    // Stats Tooltip
    private List<string> _characterStatValues = new List<string>();
    [SerializeField] private TMP_Text _characterStatValueFieldTMP;
    

    // Traits Tooltip
    [SerializeField] private GameObject _traitParent;
    [SerializeField] private GameObject _traitElementPrefab;
    [SerializeField] private GameObject[] _traitElements = new GameObject[2];

    private void Start()
    {
        Selector s = Selector._instance;
        s.OnCharacterSelected   += UpdateTooltip;
        s.OnCharacterDeselected += HideToolTip;

        for (int i = 0; i < _traitElements.Length; i++)
        {
            _traitElements[i] = Instantiate(_traitElementPrefab);
            _traitElements[i].transform.SetParent(_traitParent.transform, false);
            _traitElements[i].SetActive(false);
        }

    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        Selector._instance.OnCharacterSelected   -= UpdateTooltip;
        Selector._instance.OnCharacterDeselected -= HideToolTip;
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

    private void HideToolTip()
    {
        _layout.SetActive(false);
    }

    
    private void UpdateTooltip(Character character)
    {
        _layout.SetActive(true);
        UpdateCharacterHeaderInfo(character);
        RebuildCharacterStatTooltip(character);
        UpdateCharacterTraits(character);
    }

    private void UpdateCharacterHeaderInfo(Character character)
    {
        ClassData classData = character.GetClassData();
        Sprite sprite = classData.classImage;
        _characterIcon.sprite = sprite;
        _characterClassName.text = classData.name;
    }

    private void UpdateCharacterTraits(Character character)
    {
        IReadOnlyList<Trait> traits = character.GetTraitManager().GetAllTraits();
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
