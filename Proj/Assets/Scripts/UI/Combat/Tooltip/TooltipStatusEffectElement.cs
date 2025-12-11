using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipStatusEffectElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _statusEffectIcon;
    [SerializeField] private TMP_Text _statusEffectTitle;
    [SerializeField] private TMP_Text _statusEffectTurns;
    
    private string _description;

    public static event Action<string, string, RectTransform> OnMouseHoverEnter;
    public static event Action OnMouseHoverExit;
        
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseHoverEnter?.Invoke(_statusEffectTitle.text, _description, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseHoverExit?.Invoke();
    }

    public void SetIcon(Sprite icon) { _statusEffectIcon.sprite = icon; }
    public void SetTitle(string title) { _statusEffectTitle.text = title; }
    public void SetDescription(string destription) { _description = destription; }
    public void SetTurns(int turns) 
    {
        _statusEffectTurns.text = "Turns: " + turns.ToString();  
    }
}
