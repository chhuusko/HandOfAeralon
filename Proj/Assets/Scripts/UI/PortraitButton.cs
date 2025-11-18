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
    }
}
