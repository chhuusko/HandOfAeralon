using UnityEngine;
[CreateAssetMenu(fileName = "Seismic Bolt", menuName = "Item/Card Data/Seismic Bolt", order = 1)]
public class SeismicBolt : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
            character.GetStatusEffectManager().AddStatusEffect(new Aftershock(2));
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = 25;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
}
