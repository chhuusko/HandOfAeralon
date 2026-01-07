using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Life Reaver", menuName = "Item/Card Data/Life Reaver", order = 1)]
public class LifeReaver : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null && character.GetFaction() == Faction.Enemy)
        {
            int damage = 50;
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            character.TakeDamage(damage);
        }

        List<Character> friendlyList = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        Character lowestHP = friendlyList[0];
        foreach (Character friendly in friendlyList)
        {
            if (friendly.Data.DerivedHealthPoints-friendly.GetCurrentHealth() > lowestHP.Data.DerivedHealthPoints - lowestHP.GetCurrentHealth())
            {
                lowestHP = friendly;
            }
        }
        lowestHP.Heal(50);
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
