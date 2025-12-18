using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Precise Strike", menuName = "Item/Card Data/Precise Strike", order = 1)]
public class PreciseStrike : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            int damage = 10;
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            character.TakeDamage(damage);
            if (character.GetCurrentHealth() <= 0)
            {
                CardHandManager.GetInstance().ChangeMana(2);
            }
        }
    }
}
