using UnityEngine;

public class PortraitButton : MonoBehaviour
{
    private CharacterData _character;

    public void SetCharacter(CharacterData character)
    {
        _character = character;
    }

    public void OnClick()
    {
        CombatUI.Instance.LoadAbilities(_character);
        CombatUI.Instance.UpdatePortraitColors(gameObject);
        Selector._instance.SetSelectedCharacter(CombatManager._instance.GetCharacterDataDict()[_character]);
    }
}
