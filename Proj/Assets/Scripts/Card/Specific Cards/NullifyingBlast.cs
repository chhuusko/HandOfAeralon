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
            List<StatusEffect> statuses = new List<StatusEffect>(character.GetStatusEffectManager().GetAllEffects());
            List<StatusEffect> debuffs = new List<StatusEffect>();
            foreach (StatusEffect status in statuses)
            {
                if (status.Data.Type == StatusEffectType.Debuff)
                {
                    debuffs.Add(status);
                    
                }
            }

            int damage = debuffs.Count * 4;
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            character.TakeDamage(damage);

            foreach (StatusEffect status in debuffs)
            {
                character.GetStatusEffectManager().RemoveStatusEffect(status);
            }
        }
    }
}
