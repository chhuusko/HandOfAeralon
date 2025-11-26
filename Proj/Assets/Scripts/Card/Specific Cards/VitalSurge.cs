using UnityEngine;

[CreateAssetMenu(fileName = "Vital Surge", menuName = "Item/Card Data/Vital Surge", order = 1)]
public class VitalSurge : Card
{
    public override void PlayCard()
    {
        Character targetCharacter = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (targetCharacter != null)
        {
            if (targetCharacter.GetCurrentHealth() < targetCharacter.GetMaxHealth() / 2)
            {
                CardHandManager.GetInstance().ChangeMana(1);
            }
            targetCharacter.Heal((int)(targetCharacter.GetMaxHealth() * 0.25f));
        }
    }
}