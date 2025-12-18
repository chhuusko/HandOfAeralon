using UnityEngine;

[CreateAssetMenu(fileName = "Handbreaker Ray", menuName = "Item/Card Data/Handbreaker Ray", order = 1)]
public class HandbreakerRay : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            int damage = (Mathf.Max(0, CardHandManager.GetInstance().GetCardsInHand().Count - 1) * 4);
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyOutgoingDamage(damage, null));
            character.TakeDamage(damage);
        }
    }
}
