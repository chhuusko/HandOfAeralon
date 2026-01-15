using UnityEngine;
[CreateAssetMenu(fileName = "Venom Harvest", menuName = "Item/Card Data/Venom Harvest", order = 1)]
public class VenomHarvest : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
            character.GetStatusEffectManager().RemoveStatusEffect(new Poison(1));
        }
        
    }
    public override int GetDamage(Character character)
    {
        int totalDamage = 0;
        if (character.GetStatusEffectManager().ContainsStatusEffect<Poison>())
        {
            StatusEffect poison = character.GetStatusEffectManager().GetStatusEffect<Poison>();

            for (int i = poison.Duration; i > 0; i--)
            {
                var data = poison.Data as IntCapData;
                if (!data)
                {
                    return 0;
                }

                var damage = data.Damage;
                totalDamage += i*damage;
            }
        }
        return Mathf.RoundToInt(2 * character.GetStatusEffectManager().ModifyIncomingDamage(totalDamage, null));

    }
}
