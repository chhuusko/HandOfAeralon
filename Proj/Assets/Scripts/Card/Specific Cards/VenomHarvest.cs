using UnityEngine;
[CreateAssetMenu(fileName = "Venom Harvest", menuName = "Item/Card Data/Venom Harvest", order = 1)]
public class VenomHarvest : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            int totalDamage = 0;
            if (character.GetStatusEffectManager().ContainsStatusEffect<Poison>())
            {
                StatusEffect poison = character.GetStatusEffectManager().GetStatusEffect<Poison>();
                
                for (int i = poison.Duration; i > 0; i--)
                {
                    totalDamage += i; 
                }
            }
            character.TakeDamage(totalDamage);
        }
    }
}
