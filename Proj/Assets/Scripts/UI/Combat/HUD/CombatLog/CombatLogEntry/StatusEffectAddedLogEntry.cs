using UnityEngine;

public class StatusEffectAddedLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        var d = (StatusEffectAddedLogData)data;

        if (d?.StatusEffect == null || !d.Target)
        {
            return;
        }

        _image.sprite = d.StatusEffect.Data.Icon;

        string statusEffectName = GameTextFormatter.StatusEffectColoredLabel(d.StatusEffect);
        string targetName = d.Target.GetFaction() == Faction.Friendly ?
            GameTextFormatter.ClassColoredName(d.Target) : GameTextFormatter.FactionColoredLabel(d.Target);
        
        _text.text = $"{targetName} gained {statusEffectName}";
    }
}
