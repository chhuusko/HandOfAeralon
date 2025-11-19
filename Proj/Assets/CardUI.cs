
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI _title, _description, _mana;
    [SerializeField] Image _frame, _image;
    public void SetUpUIElements(Card card)
    {
        _title.text = card.title;
        _description.text = card.description;
        _mana.text = "" + card.cost;
        _image.sprite = card.icon;
    }
}
