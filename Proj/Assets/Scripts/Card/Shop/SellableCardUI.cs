using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellableCardUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Card _card;

    // Start is called once before the first execution of Update after the MonoBehaviour is created'
    private bool _isHeldDown;
    private bool _isSold;
    private float _sellTime = 2f;
    private float _timeHeld = 0;
    private Image _image;

    [SerializeField] Image _fillImage;
    [SerializeField] GameObject _soldText;
    [SerializeField] TextMeshProUGUI _priceText;

    private int _cost;
    private void Awake()
    {
        _cost = Shop.GetInstance().GetRemoveCardPrice();
        _fillImage.fillAmount = 0;
        _image = GetComponent<Image>();
        _priceText.text = "<color=Yellow>" + _cost + "</color><voffset=12><space=15><sprite name=\"UI_icon_59\">";

    }
    private void Update()
    {
        if (_isHeldDown)
        {
            _timeHeld += Time.deltaTime;
            _fillImage.fillAmount = 1-(_sellTime-_timeHeld)/_sellTime;
            if (_timeHeld > _sellTime)
            {
                Sell();
                _soldText.SetActive(true);
                Destroy(this);
            }
        }
    }
    public void SetCard(Card card)
    {
        _card = card;
        GetComponent<CardUI>().SetUpUIElements(card);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _isHeldDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      
        _isHeldDown = false;
        _fillImage.fillAmount = 0;
        _timeHeld = 0;
        
        
    }
    private void Sell()
    {
        GlobalGameManager.GetInstance().GetGameData().cardList.Remove(_card);
        Shop.GetInstance().ChangeCoins(-_cost);
    }
}
