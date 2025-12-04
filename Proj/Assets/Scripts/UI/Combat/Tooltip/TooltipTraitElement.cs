using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipTraitElement : MonoBehaviour
{
    [SerializeField] private Image _traitImage;
    [SerializeField] private TMP_Text _traitTitle;
    [SerializeField] private TMP_Text _traitDescription;

    public void SetTraitIcon(Image image) { _traitImage.sprite = image.sprite; }
    public void SetTraitIcon(Sprite sprite) { _traitImage.sprite = sprite; }
    public void SetTraitTitle(string title) { _traitTitle.text = title; }
    public void SetTraitDescription(string description) { _traitDescription.text = description; }

}
