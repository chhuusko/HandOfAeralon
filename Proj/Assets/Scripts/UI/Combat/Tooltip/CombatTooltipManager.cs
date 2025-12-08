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
        AbilityButton.OnMouseHoverEnter              += ShowHoverTooltip;
        AbilityButton.OnMouseHoverExit               += HideHoverTooltip;
        
    }

    private void OnDisable()
    {
        TooltipStatusEffectElement.OnMouseHoverEnter -= ShowHoverTooltip;
        TooltipStatusEffectElement.OnMouseHoverExit  -= HideHoverTooltip;
        AbilityButton.OnMouseHoverEnter              -= ShowHoverTooltip;
        AbilityButton.OnMouseHoverExit               -= HideHoverTooltip;

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            if(_characterLayout.IsHidden())
            {
                _characterLayout.ShowCanvas();
            }
            else
            {
                _characterLayout.HideCanvas();
            }
        }
    }

    public static CombatTooltipManager GetInstance() { return _instance; }

    public CombatTooltipCharacterLayout GetCharacterLayout() { return _characterLayout; }


    public void ShowHoverTooltip(Ability ability)
    {
        _combatHoverTooltip.Show(ability.GetAbilityName(), ability.GetDescription());
    }

    public void ShowHoverTooltip(string title, string description)
    {
        _combatHoverTooltip.Show(title, description);
    }
    public void HideHoverTooltip()
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
}
