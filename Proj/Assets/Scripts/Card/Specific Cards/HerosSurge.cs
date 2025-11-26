using UnityEngine;

[CreateAssetMenu(fileName = "Heros Surge", menuName = "Item/Card Data/Heros Surge", order = 1)]

public class HerosSurge : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character targetCharacter = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (targetCharacter != null && targetCharacter.GetFaction() == Faction.Friendly)
        {
            //targetCharacter.GetStatusEffectManager().AddStatusEffect(new Haste(2));
            targetCharacter.GetStatusEffectManager().AddStatusEffect(new Empowered(2));
        }
    }
}
