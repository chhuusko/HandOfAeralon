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
                
                character.TakeDamage(GetDamage(character));
            }
            character.GetStatusEffectManager().AddStatusEffect(new Burn(2));
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
        
        if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
        {
            character.PreviewHealthChange(-GetDamage(character));
        }
    }
}
