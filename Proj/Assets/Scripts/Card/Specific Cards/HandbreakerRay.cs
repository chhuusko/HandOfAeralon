using UnityEngine;

[CreateAssetMenu(fileName = "Handbreaker Ray", menuName = "Item/Card Data/Handbreaker Ray", order = 1)]
public class HandbreakerRay : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage((CardHandManager.GetInstance().GetCardsInHand().Count)*4);
        }
    }
}
