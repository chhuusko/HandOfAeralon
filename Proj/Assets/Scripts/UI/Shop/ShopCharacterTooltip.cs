using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCharacterTooltip : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    private static ShopCharacterTooltip instance;


    // Layout
    [SerializeField] private GameObject _layout;
    [SerializeField] private Image _characterIcon;
    [SerializeField] private TMP_Text _characterClassName;
    private bool _bIsHidden = true;

    // Stats Tooltip
    private List<string> _characterStatValues = new List<string>();
    [SerializeField] private TMP_Text _characterStatValueFieldTMP;

    // Traits Tooltip
    [SerializeField] private GameObject _traitParent;
    [SerializeField] private GameObject _traitElementPrefab;
    [SerializeField] private List<GameObject> _traitElements; // You can only have 2 traits so convenient with array;

    private void Awake()
    {
        InitializeCharacterStats();
        instance = this;
    }
    public static ShopCharacterTooltip GetInstance()
    {
        return instance;
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

    public void RebuildCharacterStatTooltip(CharacterData character)
    {
        _characterStatValues[(int)CharacterStatKey.CurrentHealth] = $"{character.CurrentHealthPoints}";
        _characterStatValues[(int)CharacterStatKey.CurrentInitiative] = $"{character.BaseInitiative}";
        _characterStatValues[(int)CharacterStatKey.CurrentDamage] = $"{character.DerivedDamage}";
        _characterStatValues[(int)CharacterStatKey.CurrentMovementPoints] = $"{character.BaseMovementPoints}\n";

        _characterStatValues[(int)CharacterStatKey.BaseHealth] = $"{character.BaseHealthPoints}";
        _characterStatValues[(int)CharacterStatKey.BaseInitiative] = $"{character.BaseInitiative}";
        _characterStatValues[(int)CharacterStatKey.BaseDamage] = $"{character.BaseDamage}";
        _characterStatValues[(int)CharacterStatKey.BaseMovementPoints] = $"{character.BaseMovementPoints}";

        string stats = "";
        foreach (string value in _characterStatValues)
        {
            stats += value + "\n";
        }
        _characterStatValueFieldTMP.text = stats;
    }

    public bool IsHidden() { return _bIsHidden; }
    public void HideCanvas()
    {
        gameObject.SetActive(false);
        _bIsHidden = true;
    }

    public void ShowCanvas()
    {
        gameObject.SetActive(true);
        _bIsHidden = false;
    }
    public void UpdateTooltip(CharacterData character)
    {
        _layout.SetActive(true);
        UpdateCharacterHeaderInfo(character);
        RebuildCharacterStatTooltip(character);
        UpdateCharacterTraits(character);
    }

    private void UpdateCharacterHeaderInfo(CharacterData character)
    {
        ClassData classData = character.ClassData;
        Sprite sprite = classData.classImage;
        _characterIcon.sprite = sprite;
        _characterClassName.text = classData.name;
    }

    private void UpdateCharacterTraits(CharacterData character)
    {
        // NOTE (Calle): Each character can only have 2 traits, so if a character already has traits, don't add more i.e return.

        IReadOnlyList<Trait> traits = character.TraitManager.GetAllTraits();

        foreach (GameObject trait in _traitElements)
        {
            Destroy(trait);
        }
        _traitElements.Clear();

        for (int i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];

            GameObject newTrait = Instantiate(_traitElementPrefab);
            _traitElements.Add(newTrait);
            _traitElements[i].transform.SetParent(_traitParent.transform, false);

            TooltipTraitElement traitElementScript = _traitElements[i].GetComponent<TooltipTraitElement>();
            traitElementScript.SetTraitIcon(trait.Data.Icon);
            traitElementScript.SetTraitTitle(trait.Data.name);
            traitElementScript.SetTraitDescription(trait.Data.Description);
        }
    }

    private void UpdateTooltip(int health, GameObject character)
    {
        RebuildCharacterStatTooltip(character.GetComponent<CharacterData>());
    }

    public void ShowCharacterTooltip()
    {

    }
}
