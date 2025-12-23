using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image _image;
    [SerializeField] private Image _healthFill;
    [SerializeField] private TextMeshProUGUI _healthText, _sellText;
    [SerializeField] private CharacterData _characterData;
    [SerializeField] private int _sellPrice;

    public void SetUIElements(CharacterData characterData)
    {
        _characterData = characterData;
        _image.sprite = characterData.ClassData.classImage;
        _healthFill.fillAmount = 1f-((float)characterData.CurrentHealthPoints/ (float)characterData.DerivedHealthPoints);
        _healthText.text = "Health " + characterData.CurrentHealthPoints + "/" + characterData.DerivedHealthPoints + "<voffset=12.5> <space=3> <sprite name=\"UI_icon_104\">";
        _sellText.text = "Sell " + _sellPrice + "<voffset=12.5> <space=3> <sprite name=\"UI_icon_59\">";
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        ShopCharacterTooltip.GetInstance().ShowCanvas();
        ShopCharacterTooltip.GetInstance().UpdateTooltip(_characterData);
    }
    public void SellCharacter()
    {
        Debug.Log("SellCharacter");
        if (GlobalGameManager.GetInstance().GetGameData().heroDataList.Count > 1)
        {
            Shop.GetInstance().Bought(-_sellPrice);
            GlobalGameManager.GetInstance().GetGameData().heroDataList.Remove(_characterData);

            Shop.GetInstance().LoadParty();
        }
    }
}
