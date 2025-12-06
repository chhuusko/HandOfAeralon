using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Sacred Shielding", menuName = "Item/Card Data/Sacred Shielding", order = 1)]
public class SacredShielding : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();

        if (character != null)
        {
            List<StatusEffect> statuses = new List<StatusEffect>(character.GetStatusEffectManager().GetAllEffects());
            foreach (StatusEffect status in statuses)
            {
                if (status.Data.Type == StatusEffectType.Debuff)
                {
                    character.GetStatusEffectManager().RemoveStatusEffect(status);
                }
            }
            character.GetStatusEffectManager().AddStatusEffect(new Sanctified(2));

        }
    }
}
