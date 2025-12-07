using UnityEngine;

[CreateAssetMenu(fileName = "Windsprint", menuName = "Item/Card Data/Windsprint", order = 1)]
public class Windsprint : Card
{
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Haste(2));
        }
    }
}
