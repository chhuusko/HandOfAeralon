using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyableCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Card _card;

    // Start is called once before the first execution of Update after the MonoBehaviour is created'
    private bool _isHeldDown;
    private float _sellTime = 2f;
    private float _timeHeld = 0;
    private int _price;
    [SerializeField] Image _fillImage;
    [SerializeField] GameObject _aboveText;
    [SerializeField] TextMeshProUGUI _priceText;
    private void Awake()
    {
        _fillImage.fillAmount = 0;
        
    }
    private void Update()
    {
        if (_isHeldDown)
        {
            _timeHeld += Time.deltaTime;
            _fillImage.fillAmount = 1 - (_sellTime - _timeHeld) / _sellTime;
            if (_timeHeld > _sellTime)
            {
                Bought();
                _aboveText.SetActive(true);
                Destroy(this);
            }
        }
    }
    public void SetCard(Card card)
    {
        _card = card;
        _price = (int)(card.rarity+1)*50;
        _priceText.text = "£" + _price;
        GetComponent<CardUI>().SetUpUIElements(card);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canAfford()) return;
        _isHeldDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        _isHeldDown = false;
        _fillImage.fillAmount = 0;
        _timeHeld = 0;


    }
    private void Bought()
    {
        GlobalGameManager.GetInstance().GetGameData().cardList.Add(_card);
        Shop.GetInstance().ChangeCoins(-_price);
    }
    private bool canAfford()
    {
        return GlobalGameManager.GetInstance().GetGameData().coins >= _price;
    }
}
