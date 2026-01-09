using UnityEngine;

[CreateAssetMenu(fileName = "Distant Tomes", menuName = "Item/Card Data/Distant Tomes", order = 1)]
public class DistantTomes : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = CardHandManager.GetInstance().GetDeck().Count* 3;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
}
