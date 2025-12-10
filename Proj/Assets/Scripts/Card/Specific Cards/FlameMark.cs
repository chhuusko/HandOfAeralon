using UnityEngine;

[CreateAssetMenu(fileName = "Flame Mark", menuName = "Item/Card Data/Flame Mark", order = 1)]
public class FlameMark : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            
            if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
            {
                character.TakeDamage(15);
            }
            character.GetStatusEffectManager().AddStatusEffect(new Burn(null, 2));
        }
    }
}
