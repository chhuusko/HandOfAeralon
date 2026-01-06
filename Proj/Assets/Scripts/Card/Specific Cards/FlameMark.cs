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
                int damage = 15;
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
                character.TakeDamage(damage);
            }
            character.GetStatusEffectManager().AddStatusEffect(new Burn(2));
        }
    }
}
