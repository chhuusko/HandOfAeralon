using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCharacterTooltip : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private enum ShopCharacterStatKey
    {
        Health,
        CurrentDamage,
        CurrentInitiative,
        CurrentMovementPoints
    };

    private static ShopCharacterTooltip instance;


    // Layout
    [SerializeField] private GameObject _layout;
    [SerializeField] private Image _characterIcon;
    [SerializeField] private TMP_Text _characterClassName;
    [SerializeField] private TMP_Text _characterName;
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
        HideCanvas();
    }
    public static ShopCharacterTooltip GetInstance()
    {
        return instance;
    }
    public void InitializeCharacterStats()
    {
        _characterStatValues.Insert((int)ShopCharacterStatKey.Health, "");
        _characterStatValues.Insert((int)ShopCharacterStatKey.CurrentDamage, "");
        _characterStatValues.Insert((int)ShopCharacterStatKey.CurrentInitiative, "");
        _characterStatValues.Insert((int)ShopCharacterStatKey.CurrentMovementPoints, "");

        _characterStatValueFieldTMP.text = "";

        foreach (string value in _characterStatValues)
        {
            _characterStatValueFieldTMP.text += value + "\n";
        }
    }

    public void RebuildCharacterStatTooltip(CharacterData character)
    {
        _characterStatValues[(int)ShopCharacterStatKey.Health] = $"{character.CurrentHealthPoints} / {character.DerivedHealthPoints}";
        _characterStatValues[(int)ShopCharacterStatKey.CurrentDamage] = $"{character.DerivedDamage}";
        _characterStatValues[(int)ShopCharacterStatKey.CurrentInitiative] = $"{character.BaseInitiative}";
        _characterStatValues[(int)ShopCharacterStatKey.CurrentMovementPoints] = $"{character.BaseMovementPoints}";


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
        Sprite sprite = classData.friendlyImage;
        _characterIcon.sprite = sprite;
        _characterClassName.text = GameTextFormatter.CharacterColoredLabel(character, classData.name);
        _characterName.text = GameTextFormatter.CharacterColoredLabel(character, character.Name);
    }

    private void UpdateCharacterTraits(CharacterData character)
    {
        // NOTE (Calle): Each character can only have 2 traits, so if a character already has traits, don't add more i.e return.

        IReadOnlyList<Trait> traits = character.StatusEffectCollection.GetAllTraits();

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
            traitElementScript.SetTraitDescription(trait.GetColorCodedDescription());
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
