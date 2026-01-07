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
        
        string cardName = TextMarkupExtensions.Colorize(d.Card.title, ColorDatabase.Instance.CardColor);
        string targetName = GetCharacterIdentifier(d.Target);
        _text.text = $"Used {cardName} on {targetName}";
    }
}
