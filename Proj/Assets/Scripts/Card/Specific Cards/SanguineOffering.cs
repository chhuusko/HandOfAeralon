using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sanguine Offering", menuName = "Item/Card Data/Sanguine Offering", order = 1)]
public class SanguineOffering : Card
{
    public override void PlayCard()
    {
        Character targetCharacter = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (targetCharacter != null && targetCharacter.GetFaction() == Faction.Friendly)
        {
            targetCharacter.TakeDamage(10);
        }
        List<Character> characterList = CombatGrid._instance.GetAllCharacterScripts();
        foreach (Character character in characterList)
        {
            if (character.GetFaction() == Faction.Friendly && character != targetCharacter)
            {
                character.Heal(10);
            }
        }
    }
}
