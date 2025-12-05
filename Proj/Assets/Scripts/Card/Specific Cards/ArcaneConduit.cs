using UnityEngine;
[CreateAssetMenu(fileName = "Arcane Conduit", menuName = "Item/Card Data/Arcane Conduit", order = 1)]
public class ArcaneConduit : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();

        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new ConduitOfPower());

        }
    }
}
