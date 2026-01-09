using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nullifying Blast", menuName = "Item/Card Data/Nullifying Blast", order = 1)]
public class NullifyingBlast : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            
            character.TakeDamage(GetDamage(character));

            List<StatusEffect> statuses = new List<StatusEffect>(character.GetStatusEffectManager().GetAllEffectsSnapshot());
            List<StatusEffect> debuffs = new List<StatusEffect>();
            foreach (StatusEffect status in statuses)
            {
                if (status.Data.Type == StatusEffectType.Debuff)
                {
                    debuffs.Add(status);

                }
            }
            foreach (StatusEffect status in debuffs)
            {
                character.GetStatusEffectManager().RemoveStatusEffect(status);
            }
        }
    }
    public override int GetDamage(Character character)
    {
        List<StatusEffect> statuses = new List<StatusEffect>(character.GetStatusEffectManager().GetAllEffectsSnapshot());
        List<StatusEffect> debuffs = new List<StatusEffect>();
        foreach (StatusEffect status in statuses)
        {
            if (status.Data.Type == StatusEffectType.Debuff)
            {
                debuffs.Add(status);

            }
        }
        int damage = debuffs.Count * 20;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
}
