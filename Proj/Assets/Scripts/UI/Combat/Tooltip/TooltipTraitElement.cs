using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipTraitElement : MonoBehaviour
{
    [SerializeField] private Image _traitImage;
    [SerializeField] private TMP_Text _traitTitle;
    [SerializeField] private TMP_Text _traitDescription;

    public void SetTraitImage(Image image) { _traitImage = image; }
    public void SetTraitTitle(string title) { _traitTitle.text = title; }
    public void SetTraitDescription(string description) { _traitDescription.text = description; }

}
