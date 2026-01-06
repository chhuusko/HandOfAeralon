using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Life And Death", menuName = "Item/Card Data/Life And Death", order = 1)]
public class LifeAndDeath : Card
{
    public override void PlayCardOnTarget(Character character)
    {        
        List<Character> allies = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        
        int damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage((int)(character.GetMaxHealth()*0.25f), null));
        int heal = Mathf.Min(character.GetCurrentHealth(), damage);
        character.TakeDamage(damage);

        Character lowestHp = allies[0];

        foreach (Character ally in allies)
        {
            if (ally.Data.DerivedHealthPoints/ally.GetCurrentHealth() > lowestHp.Data.DerivedHealthPoints/lowestHp.GetCurrentHealth())
            {
                lowestHp = ally;
            }
        }

        lowestHp.Heal(heal);
    }

    
}
