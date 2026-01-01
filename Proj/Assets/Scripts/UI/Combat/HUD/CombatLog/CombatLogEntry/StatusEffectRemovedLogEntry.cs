using UnityEngine;

public class StatusEffectRemovedLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        var d = (StatusEffectRemovedLogData)data;

        if (d?.StatusEffect == null || !d.Character)
        {
            return;
        }
        
        _image.sprite = d.StatusEffect.Data.Icon;

        string statusEffectName = GameTextFormatter.StatusEffectColoredLabel(d.StatusEffect);
        string name = d.Character.GetFaction() == Faction.Friendly ?
            GameTextFormatter.ClassColoredName(d.Character) : GameTextFormatter.FactionColoredLabel(d.Character);
        
        _text.text = $"{name} lost {statusEffectName}";
    }
}
