using UnityEngine;

[CreateAssetMenu(fileName = "Library Crash", menuName = "Item/Card Data/Library Crash", order = 1)]

public class LibraryCrash : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        
        if (character != null)
        {
            int damage = 10;
            if (CardHandManager.GetInstance().GetDeck().Count < CardHandManager.GetInstance().GetDiscardPile().Count)
            {
                damage = 25;
            }
            
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            character.TakeDamage(damage);
            
            
        }
    }
}
