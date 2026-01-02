using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PortraitButton))]
public class ActiveTurnCharacterButton : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Image _portrait;
    [SerializeField] private PortraitButton _portraitButton;
    
    [Header("Colors")]
    [SerializeField] private Color _friendlyColor;
    [SerializeField] private Color _enemyColor;

    public void SetActiveCharacter(Character character)
    {
        if (character == null)
        {
            _portrait.sprite = null;
            _portrait.gameObject.SetActive(false);
            return;
        }
        
        _portrait.gameObject.SetActive(true);
        _portrait.sprite = character.GetClassData().classImage;
        _portrait.color = character.GetFaction() == Faction.Friendly ? _friendlyColor : _enemyColor;

        if (_portraitButton != null)
        {
            _portraitButton.Character = character;
        }
    }
}
