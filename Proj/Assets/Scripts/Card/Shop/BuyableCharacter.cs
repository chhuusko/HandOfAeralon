using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyableCharacter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private CharacterData _characterData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created'
    private bool _isHeldDown;
    private float _sellTime = 2f;
    private float _timeHeld = 0;
    private int price = 100;
    [SerializeField] private Image portrait;
    [SerializeField] private Image _fillImage;
    [SerializeField] private GameObject _aboveText;
    [SerializeField] private TextMeshProUGUI _priceText;
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
    public void SetCharacter(CharacterData characterData)
    {
        portrait.sprite = characterData.ClassData.friendlyImage;
        _characterData = characterData;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!RoomInParty() || !CanAfford()) return;
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
        GlobalGameManager.GetInstance().GetGameData().heroDataList.Add(_characterData);
        Shop.GetInstance().LoadParty();
        Shop.GetInstance().Bought(price);
    }
    private bool CanAfford()
    {
        return GlobalGameManager.GetInstance().GetGameData().coins >= price;
    }
    private bool RoomInParty()
    {
        return GlobalGameManager.GetInstance().GetGameData().heroDataList.Count < 4;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        ShopCharacterTooltip.GetInstance().ShowCanvas();
        ShopCharacterTooltip.GetInstance().UpdateTooltip(_characterData);
    }
}
