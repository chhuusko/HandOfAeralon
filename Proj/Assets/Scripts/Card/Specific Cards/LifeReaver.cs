using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Life Reaver", menuName = "Item/Card Data/Life Reaver", order = 1)]
public class LifeReaver : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character targetCharacter = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(10);
        }

        List<Character> characterList = CombatGrid._instance.GetAllCharacterScripts();
        Character lowestHP = characterList[0];
        foreach (Character character in characterList)
        {
            if (character.GetCurrentHealth() < lowestHP.GetCurrentHealth())
            {
                lowestHP = character;
            }
        }
        lowestHP.Heal(10);
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
