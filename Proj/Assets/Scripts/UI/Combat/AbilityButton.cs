using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<AbilityButton, Ability> OnMouseHoverEnter;
    public static event Action OnMouseHoverExit;
    
    public Ability Ability { get; set; }
    [SerializeField] private Button _button;
    public Button Button => _button;
    
    [SerializeField] private TMP_Text _cooldownText;
    public TMP_Text CooldownText => _cooldownText;
    
    public void OnClick()
    {
        Selector._instance.PreviewAbilityRange(Ability);
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

    public void SetCooldownTextActive(bool active)
    {
        _cooldownText.gameObject.SetActive(active);
    }

    public void SetCooldownText(int cooldown)
    {
        _cooldownText.text = cooldown.ToString();
    }
}
