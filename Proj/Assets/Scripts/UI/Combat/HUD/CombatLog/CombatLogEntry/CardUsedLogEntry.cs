using UnityEngine;

public class CardUsedLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        var d = (CardUsedLogData)data;

        if (d == null || !d.Card)
        {
            return;
        }
        
        _image.sprite = d.Card.icon;

        string cardName = TextMarkupExtensions.Colorize(d.Card.title, ColorDatabase.Instance.CardColor);
        
        _text.text = $"Used {cardName}";
    }
}
