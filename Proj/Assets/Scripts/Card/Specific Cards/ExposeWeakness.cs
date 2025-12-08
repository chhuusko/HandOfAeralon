using UnityEngine;

[CreateAssetMenu(fileName = "Expose Weakness", menuName = "Item/Card Data/Expose Weakness", order = 1)]

public class ExposeWeakness : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Vulnerable(2));
        }
    }
}
