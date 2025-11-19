using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyableCharacter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private CharacterData _characterData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created'
    private bool _isHeldDown;
    private float _sellTime = 2f;
    private float _timeHeld = 0;
    [SerializeField] Image _fillImage;
    [SerializeField] GameObject _aboveText;
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
        GetComponent<Image>().sprite = characterData.ClassData.classImage;
        _characterData = characterData;
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
    private void Bought()
    {
        GlobalGameManager.GetInstance().GetGameData().heroDataList.Add(_characterData);
        Shop.GetInstance().ChangeCoins(-10);
    }
}
