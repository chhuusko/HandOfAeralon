using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sanguine Offering", menuName = "Item/Card Data/Sanguine Offering", order = 1)]
public class SanguineOffering : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        
        if (character != null && character.GetFaction() == Faction.Friendly)
        {
            character.TakeDamage(10);
        }
        List<Character> friendlyList = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        foreach (Character friendly in friendlyList)
        {
            if (friendly.GetFaction() == Faction.Friendly && friendly != character)
            {
                friendly.Heal(10);
            }
        }
    }
}
