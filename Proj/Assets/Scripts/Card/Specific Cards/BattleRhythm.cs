using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle Rhythm", menuName = "Item/Card Data/Battle Rhythm", order = 1)]

public class BattleRhythm : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damageIncrease = 25;
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = CardHandManager.GetInstance().GetCardsPlayedThisTurn() * damageIncrease;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
}
