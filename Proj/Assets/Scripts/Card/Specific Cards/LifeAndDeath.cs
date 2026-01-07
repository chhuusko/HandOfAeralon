using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Life And Death", menuName = "Item/Card Data/Life And Death", order = 1)]
public class LifeAndDeath : Card
{
    public override void PlayCardOnTarget(Character character)
    {        
        List<Character> allies = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        int heal = Mathf.Min(character.GetCurrentHealth(), GetDamage(character));
        character.TakeDamage(GetDamage(character));

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
    public override int GetDamage(Character character)
    {
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage((int)(character.GetMaxHealth() * 0.25f), null));
    }
    public override void ShowDamagePreview(Character character)
    {
        List<Character> allies = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        int heal = Mathf.Min(character.GetCurrentHealth(), GetDamage(character));
        character.PreviewHealthChange(GetDamage(character));
        Character lowestHp = allies[0];

        foreach (Character ally in allies)
        {
            if (ally.Data.DerivedHealthPoints / ally.GetCurrentHealth() > lowestHp.Data.DerivedHealthPoints / lowestHp.GetCurrentHealth())
            {
                lowestHp = ally;
            }
        }

        lowestHp.PreviewHealthChange(heal);
    }

    
}
