using UnityEngine;
using UnityEngine.EventSystems;

public class CardPackCard : MonoBehaviour, IPointerClickHandler
{
    private bool isSelected;
    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;
    }
}
