using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Mend Wounds", menuName = "Item/Card Data/Mend Wounds", order = 1)]
public class MendWounds : Card
{
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            if ((character.GetCurrentHealth() / character.GetMaxHealth()) < 0.5f)
            {
                CardHandManager.GetInstance().ChangeMana(1);
            }
            character.Heal(50);
        }
    }
    public override void ShowDamagePreview(Character character)
    {
        character.PreviewHealthChange(50);
    }
}
