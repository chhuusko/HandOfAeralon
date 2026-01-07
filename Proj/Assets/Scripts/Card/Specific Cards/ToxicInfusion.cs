using UnityEngine;

[CreateAssetMenu(fileName = "Toxic Infusion", menuName = "Item/Card Data/Toxic Infusion", order = 1)]
public class ToxicInfusion : Card
{
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            
            if (character.GetStatusEffectManager().ContainsStatusEffect<Poison>())
            {
                character.TakeDamage(GetDamage(character));
            }
            character.GetStatusEffectManager().AddStatusEffect(new Poison(3));

        }
    }
    public override int GetDamage(Character character)
    {
        int damage = 70;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
    public override void ShowDamagePreview(Character character)
    {
        if (character == null) return;

        if (character.GetStatusEffectManager().ContainsStatusEffect<Poison>())
        {
            character.PreviewHealthChange(-GetDamage(character));
        }
    }
}
