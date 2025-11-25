using UnityEngine;
[CreateAssetMenu(fileName = "Surging Might", menuName = "Item/Card Data/Surging Might", order = 1)]
public class SurgingMight : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            character.GetComponent<StatusEffectManager>().AddStatusEffect(new Empowered(2));
        }

    }
}
