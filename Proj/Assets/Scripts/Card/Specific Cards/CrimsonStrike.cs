using UnityEngine;

[CreateAssetMenu(fileName = "Crimson Strike", menuName = "Item/Card Data/Crimson Strike", order = 1)]
public class CrimsonStrike : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage((int)(character.GetMaxHealth() * 0.25f));
            if (character.GetCurrentHealth() <= 0)
            {
                CardHandManager.GetInstance().ChangeMana(2);
            }
        }
    }
}
