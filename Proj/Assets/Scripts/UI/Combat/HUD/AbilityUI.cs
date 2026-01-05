using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Button _abilityButtonPrefab;
    [SerializeField] private Image _abilityPanel;
    private List<AbilityButton> _abilityButtons = new();
    
    [SerializeField] private GameObject[] panels;
    
    private AbilityButton _selectedAbility;

    private void OnEnable()
    {
        CombatEventManager.OnAbilityCast += UpdateAbilityButton;
        CombatEventManager.OnCharacterMove += CharacterMoving;
        CombatEventManager.OnEnterCombatStatePlaceCharacter += LoadAbilities;
        CombatEventManager.OnAbilityCast += DeactivateBorder;

        StartCoroutine(WaitForSelector());
    }

    private IEnumerator WaitForSelector()
    {
        while (!Selector._instance)
        {
            yield return null;
        }
        
        Selector._instance.OnCharacterDeselected += DeactivateBorder;
    }

    public void SetPanelsActive(bool active)
    {
        foreach (var panel in panels)
        {
            panel.SetActive(active);
        }

        if (CombatUI.Instance != null)
        {
            CombatUI.Instance._movementPointsPanel.SetActive(active);
        }
    }

    public void ClearAbilityButtons()
    {
        _selectedAbility = null;
        _abilityButtons.Clear();
        
        // Remove all current ability buttons.
        for (int i = 0; i < _abilityPanel.transform.childCount; i++)
        {
            Destroy(_abilityPanel.transform.GetChild(i).gameObject);
        }
        
        SetPanelsActive(false);
    }

    private void LoadAbilities()
    {
        LoadAbilities(CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter());
    }
    
    /// <summary>
    /// Displays each available ability for the selected character.
    /// </summary>
    /// <param name="portraitButton">The character of which's abilities to display.</param>
    public void LoadAbilities(PortraitButton portraitButton)
    {
        LoadAbilities(portraitButton.Character);
    }

    public void LoadAbilities(Character c)
    {
        LoadAbilities(c.Data);
    }
    
    public void LoadAbilities(CharacterData character)
    {
        if (!CombatUI.Instance.bCombatStarted)
        {
            return;
        }
        
        StartCoroutine(LoadAbilitiesNextFrame(character));
    }

    /// <summary>
    /// Loads all the given characters abilities and adds them to the UI.
    /// </summary>
    /// <param name="character">The character of which's abilities to load.</param>
    /// <returns></returns>
    private IEnumerator LoadAbilitiesNextFrame(CharacterData character)
    {
        yield return null;
        
        if (character == null)
        {
            DebugLog.JoppaLog("No selected character");
            yield break;
        }

        // Don't show abilities for enemies.
        if (character.Faction == Faction.Enemy)
        {
            yield break;
        }

        if (!CombatUI.Instance.bCombatStarted)
        {
            DebugLog.JoppaLog("Combat not started");
            yield break;
        }

        ClearAbilityButtons();
        SetPanelsActive(true);

        for (int i = 0; i < character.Abilities.Count; i++)
        {
            var buttonGO = Instantiate(_abilityButtonPrefab.gameObject);
            buttonGO.SetActive(false);
            buttonGO.transform.SetParent(_abilityPanel.transform, false);
            
            var button = buttonGO.GetComponent<Button>();
            
            var ability = character.Abilities[i];
            button.image.sprite = ability.GetIcon();
            button.GetComponent<AbilityButton>().Ability = ability;
            button.GetComponent<AbilityButton>().OnAbilityButtonClicked += SetBorder;
            
            var abilityButton = button.GetComponent<AbilityButton>();
            _abilityButtons.Add(abilityButton);
            
            UpdateAbilityButton(CombatManager._instance.GetCharacterDataDict()[character], abilityButton);
            
            // Only set the button as active after fully creating it.
            buttonGO.SetActive(true);
        }
    }

    public void UpdateAbilityButton()
    {
        if (CombatUI.Instance.SelectedCharacter == null)
        {
            return;
        }
        
        var dict = CombatManager._instance.GetCharacterDataDict();
        if (dict.TryGetValue(CombatUI.Instance.SelectedCharacter, out var character))
        {
            foreach (var abilityButton in _abilityButtons)
            {
                UpdateAbilityButton(character, abilityButton);
            }
        }
    }

    /// <summary>
    /// Sets the button as interactable depending on game state.
    /// </summary>
    /// <param name="c">The current character.</param>
    /// <param name="abilityButton">The ability button to set.</param>
    private void UpdateAbilityButton(Character c, AbilityButton abilityButton)
    {
        if (abilityButton == null || c == null)
        {
            Debug.LogWarning("abilityButton or character is null");
            return;
        }
    
        bool interactable = false;

        if (CombatUI.Instance.bCombatStarted && c && CombatUI.Instance.CurrentTurnCharacter && CombatUI.Instance.SelectedCharacter != null)
        {
            bool isTurnCharacter = c == CombatUI.Instance.CurrentTurnCharacter;
            bool isFriendly = CombatUI.Instance.SelectedCharacter.Faction == Faction.Friendly;
            bool notOnCooldown = !c.IsAbilityCooldownActive(abilityButton.Ability);
            bool canUseAbility = c.CanUseAbility;
            bool isActiveAbility = c.Data.ActiveAbilities.Contains(abilityButton.Ability);
            bool notStunned = !c.IsStunned;

            interactable = isTurnCharacter && isFriendly && notOnCooldown && canUseAbility && isActiveAbility && notStunned;
        }

        abilityButton.Button.interactable = interactable;

        StartCoroutine(SetCooldown(c, abilityButton));
    }

    /// <summary>
    /// Sets the buttons cooldown text.
    /// </summary>
    /// <param name="c">The current character.</param>
    /// <param name="abilityButton">The button to update.</param>
    /// <returns></returns>
    private IEnumerator SetCooldown(Character c, AbilityButton abilityButton)
    {
        yield return null;

        if (abilityButton == null || abilityButton.CooldownText == null)
        {
            yield break;
        }

        if (!c || !c.IsAbilityCooldownActive(abilityButton.Ability))
        {
            yield break;
        }
        abilityButton.SetCooldownTextActive(true);
        abilityButton.SetCooldownText(c.GetCurrentCooldown(abilityButton.Ability));
    }
    
    /// <summary>
    /// Sets all buttons as active or inactive, depending on if the character is moving currently.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <param name="moving">Whether the character is moving.</param>
    private void CharacterMoving(Character character, bool moving)
    {
        if (moving)
        {
            // Deactivate all buttons.
            foreach (var abilityButton in _abilityButtons)
            {
                abilityButton.Button.interactable = false;
            }
        }
        else
        {
            // Check state.
            UpdateAbilityButton();
        }
    }

    private void DeactivateBorder()
    {
        foreach (var abilityButton in _abilityButtons)
        {
            abilityButton.SetBorder(false);
        }
    }

    private void SetBorder(AbilityButton abilityButton)
    {
        if (abilityButton == _selectedAbility)
        {
            return;
        }

        if (_selectedAbility != null)
        {
            _selectedAbility.SetBorder(false);
        }
        
        _selectedAbility = abilityButton;
        _selectedAbility.SetBorder(true);
    }

    private void OnDisable()
    {
        CombatEventManager.OnAbilityCast -= UpdateAbilityButton;
        CombatEventManager.OnCharacterMove -= CharacterMoving;
        CombatEventManager.OnEnterCombatStatePlaceCharacter -= LoadAbilities;
        CombatEventManager.OnAbilityCast -= DeactivateBorder;
        Selector._instance.OnCharacterDeselected -= DeactivateBorder;

        foreach (var abilityButton in _abilityButtons)
        {
            abilityButton.GetComponent<AbilityButton>().OnAbilityButtonClicked -= SetBorder;
        }
    }
}
