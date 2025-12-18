using UnityEngine;
using UnityEngine.EventSystems;

public class CardPackCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _outLine;
    [SerializeField] public static readonly int maxSelect = 2;
    private static int selectedCount = 0;
    private bool _isSelected;
    private Card _card;
    private void Awake()
    {
        Shop.GetInstance().GetCardPack().UpdateText();
        _outLine.SetActive(_isSelected);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            if (selectedCount < maxSelect)
            {
                
                _isSelected = true;
                selectedCount++;
            }
            
        } else
        {
            _isSelected = false;
            selectedCount--;
        }
        _outLine.SetActive(_isSelected);
        Shop.GetInstance().GetCardPack().UpdateText();
    }
    private void OnDestroy()
    {
        if (_isSelected)
        {
            selectedCount = 0;
            GlobalGameManager.GetInstance().GetGameData().cardList.Add(_card);
        }
    }
    public void SetCard(Card card)
    {
        _card = card;
    }
    public static int GetSelectedCount()
    {
        return selectedCount;
    }
}
