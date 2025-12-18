using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Crimson Strike", menuName = "Item/Card Data/Crimson Strike", order = 1)]
public class CrimsonStrike : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            int damage = (int)(character.GetMaxHealth() * 0.25f);
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyOutgoingDamage(damage, null));
            character.TakeDamage(damage);

            if (character.GetCurrentHealth() <= 0)
            {
                CardHandManager.GetInstance().ChangeMana(2);
            }
        }
    }
}
