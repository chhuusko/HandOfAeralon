using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PortraitButton))]
public class ActiveTurnCharacterButton : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Image _portrait;
    [SerializeField] private PortraitButton _portraitButton;

    /// <summary>
    /// Updates button to reflect the currently active character in turn order.
    /// </summary>
    /// <param name="character">The character which turn it is.</param>
    public void SetCharacter(Character character)
    {
        if (character == null)
        {
            _portrait.sprite = null;
            _portrait.gameObject.SetActive(false);
            return;
        }
        
        _portrait.gameObject.SetActive(true);
        _portrait.sprite = character.GetFaction() == Faction.Friendly ? 
            character.GetClassData().friendlyImage : character.GetClassData().enemyImage;

        if (_portraitButton != null)
        {
            _portraitButton.Character = character;
        }
    }
}
