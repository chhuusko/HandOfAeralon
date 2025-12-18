using UnityEngine;

[CreateAssetMenu(fileName = "Library Crash", menuName = "Item/Card Data/Library Crash", order = 1)]

public class LibraryCrash : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        int damage = 30 - CardHandManager.GetInstance().GetDeck().Count;
        if (character != null)
        {
            if(damage >= 0)
            {
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyOutgoingDamage(damage, null));
                character.TakeDamage(damage);
            }
            else
            {
                character.TakeDamage(0);
            }
        }
    }
}
