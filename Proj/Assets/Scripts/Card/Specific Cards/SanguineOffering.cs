using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sanguine Offering", menuName = "Item/Card Data/Sanguine Offering", order = 1)]
public class SanguineOffering : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        
        if (character != null && character.GetFaction() == Faction.Friendly)
        {
            character.TakeDamage(character.GetDamage());
        }
        List<Character> friendlyList = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Friendly);
        foreach (Character friendly in friendlyList)
        {
            if (friendly.GetFaction() == Faction.Friendly && friendly != character)
            {
                friendly.Heal(50);
            }
        }
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
        foreach (Character friendly in friendlyList)
        {
            if (friendly.GetFaction() == Faction.Friendly && friendly != character)
            {
                friendly.PreviewHealthChange(50);
            }
        }
    }
}
