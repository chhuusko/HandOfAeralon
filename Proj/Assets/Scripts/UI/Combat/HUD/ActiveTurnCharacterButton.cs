using UnityEngine;
using UnityEngine.UI;

public class ActiveTurnCharacterButton : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Image _portrait;
    
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
    }
}
