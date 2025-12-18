using UnityEngine;
[CreateAssetMenu(fileName = "Flare Collapse", menuName = "Item/Card Data/Flare Collapse", order = 1)]
public class FlareCollapse : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
            {
                StatusEffect burn = character.GetStatusEffectManager().GetStatusEffect<Burn>();
                var data = burn.Data as DamageData;

                int damage = (3 * burn.Duration * data.Damage);
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
                character.TakeDamage(damage);

                character.GetStatusEffectManager().RemoveStatusEffect(burn);
            }
            
        }
    }
}
