using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Veilstep", menuName = "Item/Card Data/Veilstep", order = 1)]
public class Veilstep : Card
{
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Stealth(2));
        }
    }
}
