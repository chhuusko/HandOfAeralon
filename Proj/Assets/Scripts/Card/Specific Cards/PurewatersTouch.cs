using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Purewaters Touch", menuName = "Item/Card Data/Purewaters Touch", order = 1)]
public class PurewatersTouch : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        //TODO Needs to know what effect is debuff. needs a list of effekts
        if (character != null && character.GetFaction() == Faction.Friendly)
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
            if (debuffs.Count > 0)
            {
                character.GetStatusEffectManager().RemoveStatusEffect(debuffs[Random.Range(0, debuffs.Count)]);
                CardHandManager.GetInstance().ChangeMana(2);
            }
            character.Heal(70);
        }
    }
}
