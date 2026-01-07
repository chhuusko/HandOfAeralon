using UnityEngine;

public class StatusEffectDamageLogEntry : CombatLogEntry
{
    public override void Initialize(CombatLogData data)
    {
        if (data is not StatusEffectDamageLogData d)
        {
            return;
        }

        if (!d.Character || d.StatusEffect == null)
        {
            return;
        }

        _image.sprite = d.StatusEffect.Data.Icon;

        string statusEffectName = GameTextFormatter.StatusEffectColoredLabel(d.StatusEffect);
        string characterName = GetCharacterIdentifier(d.Character);
        
        Color damageColor = ColorDatabase.Instance.GetStatusEffectColor(d.StatusEffect);
        string damage = TextMarkupExtensions.Colorize(d.Damage.ToString(), damageColor);
        
        _text.text = $"{characterName} took {damage} damage from {statusEffectName}";
    }
}
