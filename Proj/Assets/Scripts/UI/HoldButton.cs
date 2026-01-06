using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PartyMemberUI _partyMemberUI;
    [SerializeField] private Image _fillImage;

    private bool _isHeldDown;
    private float _timeHeld;
    private float _sellTime = 1f;
    private void Awake()
    {
        _fillImage.fillAmount = 0;
        _timeHeld = 0;
    }

    private void Update()
    {
        if (_isHeldDown)
        {
            _timeHeld += Time.deltaTime;
            _fillImage.fillAmount = 1 - (_sellTime - _timeHeld) / _sellTime;
            if (_timeHeld > _sellTime)
            {
                _partyMemberUI.SellCharacter();
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_partyMemberUI.CanSell())
        {
            _isHeldDown = true;
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHeldDown = false;
        _fillImage.fillAmount = 0;
        _timeHeld = 0;
    }
}
