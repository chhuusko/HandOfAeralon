using UnityEngine;

[CreateAssetMenu(fileName = "Handbreaker Ray", menuName = "Item/Card Data/Handbreaker Ray", order = 1)]
public class HandbreakerRay : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = (Mathf.Max(0, CardHandManager.GetInstance().GetCardsInHand().Count - 1) * 20);
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
}
