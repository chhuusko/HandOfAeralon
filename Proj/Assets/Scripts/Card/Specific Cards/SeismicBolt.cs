using UnityEngine;
[CreateAssetMenu(fileName = "Seismic Bolt", menuName = "Item/Card Data/Seismic Bolt", order = 1)]
public class SeismicBolt : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        Character character = Selector._instance.GetTileUnderMouse().GetOccupantCharacter();

        if (character != null)
        {
            character.TakeDamage(5);
            character.GetStatusEffectManager().AddStatusEffect(new Aftershock(1));

        }
    }
}
