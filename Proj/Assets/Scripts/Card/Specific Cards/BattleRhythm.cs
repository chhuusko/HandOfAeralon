using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle Rhythm", menuName = "Item/Card Data/Battle Rhythm", order = 1)]

public class BattleRhythm : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            int damage = CardHandManager.GetInstance().GetCardsPlayedThisTurn() * 5;
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            character.TakeDamage(damage);
        }
    }
}
