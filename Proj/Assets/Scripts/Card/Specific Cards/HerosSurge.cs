using UnityEngine;

[CreateAssetMenu(fileName = "Heros Surge", menuName = "Item/Card Data/Heros Surge", order = 1)]

public class HerosSurge : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null && character.GetFaction() == Faction.Friendly)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Haste(2));
            character.GetStatusEffectManager().AddStatusEffect(new Empowered(2));
        }
    }
}
