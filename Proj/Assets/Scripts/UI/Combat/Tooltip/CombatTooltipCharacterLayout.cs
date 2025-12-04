using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject[] _traitElements = new GameObject[2]; // You can only have 2 traits so convenient with array;

    // Status Effects Tooltip
    [SerializeField] private GameObject _statusEffectParent;
    [SerializeField] private GameObject _statusEffectPrefab;
    [SerializeField] private List<GameObject> _statusEffects; // Number of status effects is dynamic so convenient with a list


    private void Start()
    {
        Selector s = Selector._instance;
        s.OnCharacterSelected   += UpdateTooltip;
        s.OnCharacterDeselected += HideToolTip;
        
        // NOTE (Calle): Tooltip only needs to be updated directly if the selected character is the one
        // getting a status effect applied, otherwise it will be update when selecting the one it was applied to.
        CombatEventManager.OnStatusEffectAppliedToCharacter += UpdateSelectedCharacter;
        CombatEventManager.OnStatusEffectExpiredOnCharacter += RemoveStatusEffectOnSelectedCharacter;
        CombatEventManager.OnStatusEffectDurationChanged += UpdateSelectedCharacter;

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
        Selector._instance.OnCharacterSelected              -= UpdateTooltip;
        Selector._instance.OnCharacterDeselected            -= HideToolTip;
        CombatEventManager.OnStatusEffectAppliedToCharacter -= UpdateSelectedCharacter;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            GameObject statusEffect = Instantiate(_statusEffectPrefab);
            statusEffect.transform.SetParent(_statusEffectParent.transform, false);
            _statusEffects.Add(statusEffect);
        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            Destroy(_statusEffects.LastOrDefault());
            _statusEffects.Remove(_statusEffects.LastOrDefault());
        }
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

    private void UpdateSelectedCharacter(Character caster, Character characterSubject, StatusEffect statusEffect)
    {
        Character selectedCharacter = Selector._instance.GetSelectedCharacter();

        if(selectedCharacter == characterSubject)
        {
            UpdateTooltip(characterSubject);
        }
    }
    
    private void UpdateSelectedCharacter(Character characterSubject, StatusEffect statusEffect)
    {
        Character selectedCharacter = Selector._instance.GetSelectedCharacter();

        if(selectedCharacter == characterSubject)
        {
            UpdateTooltip(characterSubject);
        }
    }

    private void RemoveStatusEffectOnSelectedCharacter(Character characterSubject, StatusEffect status)
    {
        Character selectedCharacter = Selector._instance.GetSelectedCharacter();

        if (selectedCharacter == characterSubject)
        {
            UpdateTooltip(characterSubject);
        }
    }

    private void UpdateTooltip(Character character)
    {
        _layout.SetActive(true);
        UpdateCharacterHeaderInfo(character);
        RebuildCharacterStatTooltip(character);
        UpdateCharacterTraits(character);
        UpdateCharacterStatusEffects(character);
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
        for(int i = 0; i < traits.Count; i++)
        {
            Trait trait = _traitElements[i].GetComponent<Trait>();
            trait = traits[i];
        }
    }

    private void UpdateCharacterStatusEffects(Character character)  
    {
        IReadOnlyList<StatusEffect> statusEffects = character.GetStatusEffectManager().GetAllStatusEffects();

        // NOTE (Calle): If a character has no statuseffects, destroy and remove all effects and clear the 
        // list.
        if(statusEffects.Count == 0)
        {
            foreach(GameObject statusEffect in _statusEffects)
            {
                Destroy(statusEffect);
            }
            _statusEffects.Clear();
        }

        foreach (StatusEffect statusEffect in statusEffects) 
        {
            // NOTE (Calle): First check if the status effect exist, in that case, just set effect data on
            // each UI element.
            string statusEffectName = statusEffect.Data.name;
            Transform existingStatusEffectTransform = _statusEffectParent.transform.Find(statusEffectName);
            
            if (existingStatusEffectTransform)
            {
                GameObject existingStatusEffect = existingStatusEffectTransform.gameObject;
                StatusEffectElement existingElementScript = existingStatusEffect.GetComponent<StatusEffectElement>();
                existingElementScript.SetTurns(statusEffect.Duration);
                continue;
            }

            // NOTE (Calle): If the effect didn't exist, instantiate a new one. 
            GameObject newStatusEffectElement = Instantiate(_statusEffectPrefab.gameObject);
            StatusEffectElement elementScript = newStatusEffectElement.GetComponent<StatusEffectElement>();

            _statusEffects.Add(newStatusEffectElement);
            
            newStatusEffectElement.transform.SetParent(_statusEffectParent.transform, false);
            newStatusEffectElement.name = statusEffect.Data.name;

            
            elementScript.SetIcon(statusEffect.Data.Icon);
            elementScript.SetTitle(statusEffect.Data.name);
            elementScript.SetTurns(statusEffect.Duration);
        }

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
