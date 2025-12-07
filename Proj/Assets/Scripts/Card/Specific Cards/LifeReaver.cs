using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Life Reaver", menuName = "Item/Card Data/Life Reaver", order = 1)]
public class LifeReaver : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null && character.GetFaction() == Faction.Enemy)
        {
            character.TakeDamage(10);
        }

        List<Character> friendlyList = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        Character lowestHP = friendlyList[0];
        foreach (Character friendly in friendlyList)
        {
            if (friendly.GetCurrentHealth() < lowestHP.GetCurrentHealth())
            {
                lowestHP = friendly;
            }
        }
        lowestHP.Heal(10);
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
