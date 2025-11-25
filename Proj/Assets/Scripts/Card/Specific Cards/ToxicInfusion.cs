using UnityEngine;

[CreateAssetMenu(fileName = "Toxic Infusion", menuName = "Item/Card Data/Toxic Infusion", order = 1)]
public class ToxicInfusion : Card
{
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            
            if (character.GetStatusEffectManager().ContainsStatusEffect<Poison>())
            {
                character.TakeDamage(10);
            }
            character.GetStatusEffectManager().AddStatusEffect(new Poison(3));

        }
    }
}
