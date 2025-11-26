using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Purewaters Touch", menuName = "Item/Card Data/Purewaters Touch", order = 1)]
public class PurewatersTouch : Card
{
    public override void PlayCard()
    {
        //TODO Needs to know what effect is debuff. needs a list of effekts
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null && character.GetFaction() == Faction.Friendly)
        {
            List<StatusEffect> statuses = new List<StatusEffect>(character.GetStatusEffectManager().GetAllStatusEffects());
            List<StatusEffect> debuffs = new List<StatusEffect>();
            foreach (StatusEffect status in statuses)
            {
                //if (status.() != null)
                //{
                //
                //}
                //debuffs.Add(status);
            }
        }


            Debug.Log("TODO");
    }
    private void StatusEffectRemoved()
    {
        // give 2 mana if removed
    }


}
