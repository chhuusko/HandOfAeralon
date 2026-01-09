using UnityEngine;

public class CharacterDeathLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        var d = (CharacterDeathLogData)data;

        if (!d?.Character)
        {
            return;
        }

        string characterName = GetCharacterIdentifier(d.Character);
        _text.text = $"{characterName} died";
    }
}
