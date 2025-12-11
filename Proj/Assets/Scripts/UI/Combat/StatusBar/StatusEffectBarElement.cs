using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectBarElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    private string _description;
    private string _title;

    public static event Action<string, string, RectTransform> OnMouseHoverEnter;
    public static event Action OnMouseHoverExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseHoverEnter?.Invoke(_title, _description, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseHoverExit?.Invoke();
    }
    public void SetSpriteFromImage(Image icon) {  _icon.sprite = icon.sprite; }
    public void SetSprite(Sprite spriteIcon) { _icon.sprite = spriteIcon; }
    public void SetTitle(string title) { _title = title; }
    public void SetDescription(string description) { _description = description; }
}
