using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelect : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private GameObject _outLine;
    private int _maxSelect;
    private static int selectedCount = 0;
    private bool _isSelected;
    private Card _card;

    private void Awake()
    {
        _outLine.SetActive(_isSelected);
        _maxSelect = CardSelectHandler._instance._amount;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("card " + _card.title);
        if (!_isSelected)
        {
            if (selectedCount < _maxSelect)
            {
                _isSelected = true;
                selectedCount++;
                CardSelectHandler._instance.SetSelectCard(_card);
            }
        }
        else
        {
            _isSelected = false;
            selectedCount--;
            CardSelectHandler._instance.SetSelectCard(null);
        }
        _outLine.SetActive(_isSelected);
    }
    private void OnDestroy()
    {
        if (_isSelected)
        {
            selectedCount = 0;
        }
    }
    public void SetCard(Card card)
    {
        _card = card;
        Debug.Log("CardSet" + card);
    }
    public static int GetSelectedCount()
    {
        return selectedCount;
    }
    public void SetMaxSelect(int amount)
    {
        _maxSelect = amount;
    }
}
