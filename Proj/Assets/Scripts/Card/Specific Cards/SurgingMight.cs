using UnityEngine;
[CreateAssetMenu(fileName = "Surging Might", menuName = "Item/Card Data/Surging Might", order = 1)]
public class SurgingMight : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Empowered(2));
        }

    }
}
