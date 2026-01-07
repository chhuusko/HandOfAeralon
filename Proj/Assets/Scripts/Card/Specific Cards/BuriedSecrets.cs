using UnityEngine;

[CreateAssetMenu(fileName = "Buried Secrets", menuName = "Item/Card Data/Buried Secrets", order = 1)]

public class BuriedSecrets : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {

            int damage = (CardHandManager.GetInstance().GetDiscardPile().Count) * 10;
            damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            character.TakeDamage(damage);
        }
    }
}
