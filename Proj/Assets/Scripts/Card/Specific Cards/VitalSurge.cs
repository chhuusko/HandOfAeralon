using UnityEngine;

[CreateAssetMenu(fileName = "Vital Surge", menuName = "Item/Card Data/Vital Surge", order = 1)]
public class VitalSurge : Card
{
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            if (character.GetCurrentHealth() < character.GetMaxHealth() / 2)
            {
                CardHandManager.GetInstance().ChangeMana(1);
            }
            character.Heal((int)(character.GetMaxHealth() * 0.25f));
        }
    }
}