using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<Ability> OnMouseHoverEnter;
    public static event Action OnMouseHoverExit;
    
    public Ability Ability { get; set; }
    [SerializeField] private Button _button;
    public Button Button => _button;
    
    public void OnClick()
    {
        Selector._instance.PreviewAbilityRange(Ability);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseHoverEnter?.Invoke(Ability);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseHoverExit?.Invoke();
    }
}
