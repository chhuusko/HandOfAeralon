using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Precise Strike", menuName = "Item/Card Data/Precise Strike", order = 1)]
public class PreciseStrike : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(10);
            if (character.GetCurrentHealth() <= 0)
            {
                CardHandManager.GetInstance().ChangeMana(2);
            }
        }
    }
}
