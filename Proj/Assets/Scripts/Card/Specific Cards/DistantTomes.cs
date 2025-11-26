using UnityEngine;

[CreateAssetMenu(fileName = "Distant Tomes", menuName = "Item/Card Data/Distant Tomes", order = 1)]
public class DistantTomes : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            character.TakeDamage(CardHandManager.GetInstance().GetDeck().Count);
        }
    }
}
