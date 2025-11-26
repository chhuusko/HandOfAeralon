using UnityEngine;

[CreateAssetMenu(fileName = "Crippling Hex", menuName = "Item/Card Data/Crippling Hex", order = 1)]
public class CripplingHex : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Weakened(2));
        }
    }
}
