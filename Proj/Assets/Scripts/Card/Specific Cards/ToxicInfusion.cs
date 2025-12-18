using UnityEngine;

[CreateAssetMenu(fileName = "Toxic Infusion", menuName = "Item/Card Data/Toxic Infusion", order = 1)]
public class ToxicInfusion : Card
{
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            
            if (character.GetStatusEffectManager().ContainsStatusEffect<Poison>())
            {
                int damage = 10;
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyOutgoingDamage(damage, null));
                character.TakeDamage(damage);
            }
            character.GetStatusEffectManager().AddStatusEffect(new Poison(3));

        }
    }
}
