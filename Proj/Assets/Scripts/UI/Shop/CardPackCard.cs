using UnityEngine;
using UnityEngine.EventSystems;

public class CardPackCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _outLine;
    private bool _isSelected;
    private Card _card;
    private void Awake()
    {
        _outLine.SetActive(_isSelected);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = !_isSelected;
        _outLine.SetActive(_isSelected);
        
    }
    private void OnDestroy()
    {
        if (_isSelected)
        {
            GlobalGameManager.GetInstance().GetGameData().cardList.Add(_card);
        }
    }
    public void SetCard(Card card)
    {
        _card = card;
    }
}
