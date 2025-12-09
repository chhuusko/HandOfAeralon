using UnityEngine;
[CreateAssetMenu(fileName = "Ironhide Blessing", menuName = "Item/Card Data/Ironhide Blessing", order = 1)]

public class IronhideBlessing : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Fortified(2));
        }
    }
}
