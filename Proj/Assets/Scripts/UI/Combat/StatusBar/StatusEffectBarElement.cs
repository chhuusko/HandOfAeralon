using UnityEngine;
using UnityEngine.UI;

public class StatusEffectBarElement : MonoBehaviour
{
    [SerializeField] private Image _icon;

    public void SetSpriteFromImage(Image icon) {  _icon.sprite = icon.sprite; }
    public void SetSprite(Sprite spriteIcon) { _icon.sprite = spriteIcon; }
}
