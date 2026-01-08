using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPackUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private float _sellTime = 1f;
    private float _timeHeld = 0;
    [SerializeField] Image _fillImage;
    [SerializeField] private Image _image;
    [SerializeField] GameObject _aboveText;
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] int CardAmount;
    [SerializeField] private int _cost = 50;
    List<Card> cardInPack = new List<Card>();
    private bool _isHeldDown;
    
    [Header("Sprites")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _hoverSprite;
    [SerializeField] private Sprite _clickSprite;

    private void Awake()
    {
        RandomizeCards();
        _fillImage.fillAmount = 0;
        _priceText.text = $"<color=yellow>{_cost}</color><voffset=12><space=15><sprite index=0>";
        _image.sprite = _defaultSprite;
    }
    private void Update()
    {
        if (_isHeldDown)
        {
            _timeHeld += Time.deltaTime;
            _fillImage.fillAmount = 1 - (_sellTime - _timeHeld) / _sellTime;
            if (_timeHeld > _sellTime)
            {
                Shop.GetInstance().Bought(_cost);
                Shop.GetInstance().GetCardPack().UpdateCards(cardInPack);
                _aboveText.SetActive(true);
                Destroy(this);
            }
        }
    }
    private void RandomizeCards()
    {
        List<Card> temp = CardsUnlocked.GetInstance().GetUnlockedCards();

        for (int i = 0; i < CardAmount; i++)
        {
            cardInPack.Add(temp[Random.Range(0, temp.Count)]);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Shop.CanAfford(_cost))
        {
            _isHeldDown = true;
            _image.sprite = _clickSprite;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeldDown = false;
        _fillImage.fillAmount = 0;
        _timeHeld = 0;
        _image.sprite = _hoverSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        _image.sprite = _hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        _image.sprite = _defaultSprite;
    }
}
