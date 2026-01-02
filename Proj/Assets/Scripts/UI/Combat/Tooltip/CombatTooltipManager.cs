using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CombatTooltipManager : MonoBehaviour
{
    private static CombatTooltipManager _instance;

    [SerializeField] private CombatTooltipCharacterLayout _characterLayout;
    [SerializeField] private CombatHoverTooltip _combatHoverTooltip;

    private bool _bAbilityRequestingClose = false;
    private bool _bCloseTooltipDuringOverlap = false;
    AbilityButton _lastAbilityButton;
    Ability _lastAbility;


    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }


    void Start()
    {
        _characterLayout.InitializeCharacterStats();

        TooltipStatusEffectElement.OnMouseHoverEnter += ShowHoverTooltip;
        TooltipStatusEffectElement.OnMouseHoverExit  += HideHoverTooltip;
        AbilityButton.OnMouseHoverEnter              += ShowHoverTooltipAbility;
        AbilityButton.OnMouseHoverEnter              += AbilityOpenRequest;
        AbilityButton.OnMouseHoverExit               += HideHoverTooltip;
        AbilityButton.OnMouseHoverExit               += AbilityCloseRequest;
        StatusEffectBarElement.OnMouseHoverEnter     += ShowHoverTooltip;
        StatusEffectBarElement.OnMouseHoverExit      += HideStatusBarElementTooltip;

    }

    private void OnDisable()
    {
        TooltipStatusEffectElement.OnMouseHoverEnter -= ShowHoverTooltip;
        TooltipStatusEffectElement.OnMouseHoverExit  -= HideHoverTooltip;
        AbilityButton.OnMouseHoverEnter              -= ShowHoverTooltipAbility;
        AbilityButton.OnMouseHoverEnter              -= AbilityOpenRequest;
        AbilityButton.OnMouseHoverExit               -= HideHoverTooltip;
        AbilityButton.OnMouseHoverExit               -= AbilityCloseRequest;
        StatusEffectBarElement.OnMouseHoverEnter     -= ShowHoverTooltip;
        StatusEffectBarElement.OnMouseHoverExit      -= HideStatusBarElementTooltip;
        

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            if (_characterLayout.IsHidden())
            {
                _characterLayout.ShowCanvas();
            }
            else
            {
                _characterLayout.HideCanvas();
            }
        }
    }

    private void LateUpdate()
    {
        if(_bAbilityRequestingClose)
        {
            if (!_combatHoverTooltip.GetIsHovering() || !_combatHoverTooltip.IsLocked())
            {
                if(!_bCloseTooltipDuringOverlap)
                {
                    _combatHoverTooltip.Hide();
                    _bAbilityRequestingClose = false;
                    _bCloseTooltipDuringOverlap = true;
                }
                
            }
                
        }
    }

    public static CombatTooltipManager GetInstance() { return _instance; }

    public CombatTooltipCharacterLayout GetCharacterLayout() { return _characterLayout; }


    public void ShowHoverTooltipAbility(AbilityButton button, Ability ability)
    {

        string description = ability.GetDescription();
        description += "\n\nCooldown: " + ability.GetCooldown() + " turns.";

        string advancedDescription = GameTextFormatter.AbilityColoredLabel(ability);

        _combatHoverTooltip.Show(ability.GetAbilityName(), advancedDescription, button.GetComponent<RectTransform>());
    }

    public void ShowHoverTooltip(string title, string description, RectTransform rectTransform)
    {
        _combatHoverTooltip.Show(title, description, rectTransform);
    }

    public void HideHoverTooltip()
    {
        if(!_combatHoverTooltip.IsLocked() || !_combatHoverTooltip.GetIsHovering())
        {
            // _combatHoverTooltip.Hide();
        }
    }

    public void HideStatusBarElementTooltip()
    {
        _combatHoverTooltip.Hide();
    }

    public void HideTooltipCanvas()
    {
        if(_characterLayout != null)
        {
            _characterLayout.HideCanvas();
        }
    }

    public void ShowTooltipCanvas()
    {
        if (_characterLayout != null)
        {
            _characterLayout.ShowCanvas();
        }
    }

    public void AbilityCloseRequest()
    {
        _bAbilityRequestingClose = true;
        _bCloseTooltipDuringOverlap = false;
    }

    public void AbilityOpenRequest(AbilityButton button, Ability ability)
    {
        _lastAbilityButton = button;
        _lastAbility = ability;
        _bAbilityRequestingClose = false;

    }
}
