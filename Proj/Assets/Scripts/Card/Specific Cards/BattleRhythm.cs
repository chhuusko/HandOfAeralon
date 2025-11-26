using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle Rhythm", menuName = "Item/Card Data/Battle Rhythm", order = 1)]

public class BattleRhythm : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();

        if (character != null)
        {
            character.TakeDamage(CardHandManager.GetInstance().GetCardsPlayedThisTurn() * 5);
        }
    }
}
