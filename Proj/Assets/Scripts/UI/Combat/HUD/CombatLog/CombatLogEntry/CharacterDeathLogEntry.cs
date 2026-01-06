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
        
        string characterName = d.Character.GetFaction() == Faction.Friendly ?
            GameTextFormatter.CreateCharacterNameLink(d.Character) : GameTextFormatter.FactionColoredLabel(d.Character);
        
        _text.text = $"{characterName} died";
    }
}
