using UnityEngine;

[CreateAssetMenu(fileName = "Crimson Strike", menuName = "Item/Card Data/Crimson Strike", order = 1)]
public class CrimsonStrike : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character targetCharacter = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage((int)(targetCharacter.GetMaxHealth() * 0.25f));
        }
    }
}
