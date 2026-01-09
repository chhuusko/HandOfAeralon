using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Life Reaver", menuName = "Item/Card Data/Life Reaver", order = 1)]
public class LifeReaver : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null && character.GetFaction() == Faction.Enemy)
        {
            character.TakeDamage(GetDamage(character));
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
    public override int GetDamage(Character character)
    {
        
        int damage = 50;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
        
    }
    public override void ShowDamagePreview(Character character)
    {
        base.ShowDamagePreview(character);

        List<Character> friendlyList = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        Character lowestHP = friendlyList[0];
        foreach (Character friendly in friendlyList)
        {
            if (friendly.Data.DerivedHealthPoints - friendly.GetCurrentHealth() > lowestHP.Data.DerivedHealthPoints - lowestHP.GetCurrentHealth())
            {
                lowestHP = friendly;
            }
        }
        lowestHP.PreviewHealthChange(50);
    }
}
