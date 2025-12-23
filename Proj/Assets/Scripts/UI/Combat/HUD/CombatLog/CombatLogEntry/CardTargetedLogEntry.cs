using UnityEngine;

public class CardTargetedLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        var d = (CardTargetedLogData)data;

        if (d == null || !d.Card || !d.Target)
        {
            return;
        }
        
        _image.sprite = d.Card.icon;
        
        Color targetColor = ColorDatabase.Instance.GetCharacterColor(d.Target);

        string cardName = TextMarkupExtensions.Colorize(d.Card.title, ColorDatabase.Instance.CardColor);
        string targetName = d.Target.GetFaction() == Faction.Friendly ?
            GameTextFormatter.ClassColoredName(d.Target) : GameTextFormatter.FactionColoredLabel(d.Target);
        
        _text.text = $"Used {cardName} on {targetName}";
    }
}
