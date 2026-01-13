using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class CombatTooltipCharacterLayout : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    [SerializeField] private Animator _animatorShowHideButton;
    private Animator _animator;
    private bool _bIsHidden = true;

    // Stats Tooltip
    private List<string> _characterStatValues = new List<string>();
    [SerializeField] private TMP_Text _characterStatValueFieldTMP;
    
    // Traits Tooltip
    [SerializeField] private GameObject _traitParent;
    [SerializeField] private GameObject _traitElementPrefab;
    [SerializeField] private List<GameObject> _traitElements; // You can only have 2 traits so convenient with array;

    // Status Effects Tooltip
    [SerializeField] private GameObject _statusEffectParent;
    [SerializeField] private GameObject _statusEffectPrefab;
    [SerializeField] private List<GameObject> _statusEffects = new List<GameObject>(); // Number of status effects is dynamic so convenient with a list


    private void Start()
    {
        Selector s = Selector._instance;
        s.OnCharacterSelected   += UpdateTooltip;
        s.OnCharacterDeselected += HideToolTip;
        
        // NOTE (Calle): Tooltip only needs to be updated directly if the selected character is the one
        // getting a status effect applied, otherwise it will be update when selecting the one it was applied to.
        CombatEventManager.OnStatusEffectAppliedToCharacter += UpdateSelectedCharacter;
        CombatEventManager.OnStatusEffectExpiredOnCharacter += RemoveStatusEffectOnSelectedCharacter;
        CombatEventManager.OnStatusEffectDurationChanged    += UpdateSelectedCharacter;
        CombatEventManager.OnCharacterMove                  += UpdateTooltip;

        _animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        Selector._instance.OnCharacterSelected              -= UpdateTooltip;
        Selector._instance.OnCharacterDeselected            -= HideToolTip;

        CombatEventManager.OnStatusEffectAppliedToCharacter -= UpdateSelectedCharacter;
        CombatEventManager.OnStatusEffectExpiredOnCharacter -= RemoveStatusEffectOnSelectedCharacter;
        CombatEventManager.OnStatusEffectDurationChanged    -= UpdateSelectedCharacter;
        CombatEventManager.OnCharacterMove                  -= UpdateTooltip;
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

        _characterStatValueFieldTMP.text = "";

        foreach (string value in _characterStatValues)
        {
            _characterStatValueFieldTMP.text += value + "\n";
        }
    }

    public void RebuildCharacterStatTooltip(Character character)
    {

        _characterStatValues[(int)CharacterStatKey.CurrentHealth] = $"{character.GetCurrentHealth()}/{character.GetMaxHealth()}";
        _characterStatValues[(int)CharacterStatKey.CurrentInitiative] = $"{character.GetInitiative()}";
        _characterStatValues[(int)CharacterStatKey.CurrentDamage] = $"{character.GetDamage()}";
        _characterStatValues[(int)CharacterStatKey.CurrentMovementPoints] = $"{character.GetMovementPoints()}/{character.GetBaseMovementPoints()}\n";

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
        _animator.Play("Hide");
        _animatorShowHideButton.Play("BlinkOn");
        _bIsHidden = true;
    }

    public void ShowCanvas()
    {
        _animator.Play("Show");
        _animatorShowHideButton.Play("BlinkOff");
        _bIsHidden = false;
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
    private void UpdateTooltip(Character character, bool isMoving)
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
        _characterClassName.text = GameTextFormatter.CharacterColoredLabel(character.Data);
    }

    private void UpdateCharacterTraits(Character character)
    {
        // NOTE (Calle): Each character can only have 2 traits, so if a character already has traits, don't add more i.e return.

        IReadOnlyList<Trait> traits = character.GetTraitManager().GetAllTraits();

        foreach(GameObject trait in _traitElements)
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

    private void UpdateCharacterStatusEffects(Character character)  
    {
        IReadOnlyList<StatusEffect> statusEffects = character.GetStatusEffectManager().GetAllStatusEffectsSnapshot();

        // NOTE (Calle): If a character has no statuseffects, destroy and remove all effects and clear the 
        // list.
        if(statusEffects.Count == 0)
        {
            foreach(GameObject statusEffect in _statusEffects)
            {
                Destroy(statusEffect);
            }
            _statusEffects.Clear();
            return;
        }

        List<GameObject> toRemove = new();

        // NOTE (Calle): Remove status effects which came from the previus characte and the current doesn't have
        foreach (GameObject previousRegistererdEffect in _statusEffects)
        {
            string previousName = previousRegistererdEffect.name;

            bool didExist = false;
            foreach(StatusEffect statusEffect in statusEffects)
            {
                if(previousName.Equals(statusEffect.Name))
                {
                    didExist = true;   
                    break;
                }
            }

            if(!didExist)
            {
                toRemove.Add(previousRegistererdEffect);
            }
        }

        foreach(GameObject effectToRemove in toRemove)
        {
            _statusEffects.Remove(effectToRemove);
            Destroy(effectToRemove);
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
                TooltipStatusEffectElement existingElementScript = existingStatusEffect.GetComponent<TooltipStatusEffectElement>();
                existingElementScript.SetTurns(statusEffect.Duration);
                continue;
            }

            // NOTE (Calle): If the effect didn't exist, instantiate a new one. 
            GameObject newStatusEffectElement = Instantiate(_statusEffectPrefab.gameObject);
            TooltipStatusEffectElement elementScript = newStatusEffectElement.GetComponent<TooltipStatusEffectElement>();

            _statusEffects.Add(newStatusEffectElement);
            
            newStatusEffectElement.transform.SetParent(_statusEffectParent.transform, false);
            newStatusEffectElement.name = statusEffect.Data.name;

            
            elementScript.SetIcon(statusEffect.Data.Icon);
            elementScript.SetTitle(statusEffect.Data.name);
            elementScript.SetDescription(statusEffect.GetColorCodedDescription());
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatEventManager.InvokeOnIsHoveringUI(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatEventManager.InvokeOnIsHoveringUI(false);
    }
}
