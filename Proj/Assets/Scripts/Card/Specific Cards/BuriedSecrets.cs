using UnityEngine;

[CreateAssetMenu(fileName = "Buried Secrets", menuName = "Item/Card Data/Buried Secrets", order = 1)]

public class BuriedSecrets : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damageIncrease = 10;
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = (CardHandManager.GetInstance().GetDiscardPile().Count) * damageIncrease;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
}
