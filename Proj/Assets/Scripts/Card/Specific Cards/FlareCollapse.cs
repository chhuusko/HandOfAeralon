using UnityEngine;
[CreateAssetMenu(fileName = "Flare Collapse", menuName = "Item/Card Data/Flare Collapse", order = 1)]
public class FlareCollapse : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
            {
                StatusEffect burn = character.GetStatusEffectManager().GetStatusEffect<Burn>();
                var data = burn.Data as DamageData;
                character.TakeDamage(GetDamage(character));

                character.GetStatusEffectManager().RemoveStatusEffect(burn);
            }
            
        }
    }
    public override int GetDamage(Character character)
    {
        StatusEffect burn = character.GetStatusEffectManager().GetStatusEffect<Burn>();
        var data = burn.Data as DamageData;
        int damage = (4 * burn.Duration * data.Damage);
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));

    }
    public override void ShowDamagePreview(Character character)
    {
        if (character == null) return;
        
        if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
        {
            character.PreviewHealthChange(-GetDamage(character));
        }
    }
}
