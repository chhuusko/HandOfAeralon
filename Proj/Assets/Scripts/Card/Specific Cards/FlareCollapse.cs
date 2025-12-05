using UnityEngine;
[CreateAssetMenu(fileName = "Flare Collapse", menuName = "Item/Card Data/Flare Collapse", order = 1)]
public class FlareCollapse : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();

        if (character != null)
        {
            if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
            {
                StatusEffect burn = character.GetStatusEffectManager().GetStatusEffect<Burn>();
                var data = burn.Data as DamageData;
                character.TakeDamage(burn.Duration * data.Damage);
                character.GetStatusEffectManager().RemoveStatusEffect(burn);
            }
            
        }
    }
}
