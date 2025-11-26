using UnityEngine;

[CreateAssetMenu(fileName = "Mend Wounds", menuName = "Item/Card Data/Mend Wounds", order = 1)]
public class MendWounds : Card
{
    public override void PlayCard()
    {


        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            if ((character.GetCurrentHealth() / character.GetMaxHealth()) < 0.5f)
            {
                CardHandManager.GetInstance().ChangeMana(1);
            }
            character.Heal(10);
        }
        
        
    }
}
