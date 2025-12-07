using UnityEngine;

[CreateAssetMenu(fileName = "Windsprint", menuName = "Item/Card Data/Windsprint", order = 1)]
public class Windsprint : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Haste(2));
        }
    }
}
