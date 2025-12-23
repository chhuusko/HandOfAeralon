using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<AbilityButton, Ability> OnMouseHoverEnter;
    public static event Action OnMouseHoverExit;
    public event Action<AbilityButton> OnAbilityButtonClicked;
    
    public Ability Ability { get; set; }
    
    [Header("Button")]
    [SerializeField] private Button _button;
    public Button Button => _button;
    
    [Header("Cooldown")]
    [SerializeField] private TMP_Text _cooldownText;
    public TMP_Text CooldownText => _cooldownText;
    
    [Header("Border")]
    [SerializeField] private GameObject _border;

    public void OnClick()
    {
        Selector._instance.PreviewAbilityRange(Ability);
        OnAbilityButtonClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Ability.SetCharacterCaster(Selector._instance.GetSelectedCharacter());
        OnMouseHoverEnter?.Invoke(this, Ability);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseHoverExit?.Invoke();
    }

    public void SetBorder(bool active)
    {
        _border.SetActive(active);
    }

    public void SetCooldownTextActive(bool active)
    {
        _cooldownText.gameObject.SetActive(active);
    }

    public void SetCooldownText(int cooldown)
    {
        _cooldownText.text = cooldown.ToString();
    }
}
