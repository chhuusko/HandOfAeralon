using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Image _image;
    [SerializeField] Image _healthFill;
    [SerializeField] TextMeshProUGUI _healthText;

    public void SetUIElements(CharacterData characterData)
    {
        _image.sprite = characterData.ClassData.classImage;
        _healthFill.fillAmount = 1f-((float)characterData.CurrentHealthPoints/ (float)characterData.DerivedHealthPoints);
        _healthText.text = characterData.CurrentHealthPoints + "/" + characterData.BaseHealthPoints + "<voffset=12.5> <space=3> <sprite name=\"UI_icon_104\">";
    }
}
