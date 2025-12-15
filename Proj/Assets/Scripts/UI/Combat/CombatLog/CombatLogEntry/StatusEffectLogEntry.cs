using UnityEngine;

public class StatusEffectLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        var d = (StatusEffectLogData)data;

        if (d?.StatusEffect == null || !d.Target)
        {
            return;
        }

        _image.sprite = d.StatusEffect.Data.Icon;

        string statusEffectName = GameTextFormatter.StatusEffectColoredLabel(d.StatusEffect);
        string targetName = GameTextFormatter.FactionColoredLabel(d.Target);
        
        _text.text = $"{targetName} gained {statusEffectName}";
    }
}
