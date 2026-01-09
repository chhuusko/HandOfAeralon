using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Precise Strike", menuName = "Item/Card Data/Precise Strike", order = 1)]
public class PreciseStrike : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            
            character.TakeDamage(GetDamage(character));
            if (character.GetCurrentHealth() <= 0)
            {
                CardHandManager.GetInstance().ChangeMana(2);
            }
        }
        
    }
    public override int GetDamage(Character character)
    {
        int damage = 50;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
        
    }
}
