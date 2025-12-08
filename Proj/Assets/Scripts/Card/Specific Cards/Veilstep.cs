using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Veilstep", menuName = "Item/Card Data/Veilstep", order = 1)]
public class Veilstep : Card
{
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Stealth(2));
        }
    }
}
