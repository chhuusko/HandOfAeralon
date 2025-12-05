using UnityEngine;

[CreateAssetMenu(fileName = "Flame Mark", menuName = "Item/Card Data/Flame Mark", order = 1)]
public class FlameMark : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();
        if (character != null)
        {
            character.GetStatusEffectManager().AddStatusEffect(new Burn(null, 2));
            if (character.GetStatusEffectManager().ContainsStatusEffect<Burn>())
            {
                character.TakeDamage(15);
            }
        }
    }
}
