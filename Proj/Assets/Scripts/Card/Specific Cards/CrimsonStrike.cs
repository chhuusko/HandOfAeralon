using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Crimson Strike", menuName = "Item/Card Data/Crimson Strike", order = 1)]
public class CrimsonStrike : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] float healthProcentage = 0.25f;
    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {

            character.TakeDamage(GetDamage(character));

            if (character.GetCurrentHealth() <= 0)
            {
                CardHandManager.GetInstance().ChangeMana(2);
            }
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = (int)(character.GetMaxHealth() * healthProcentage);
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
        
    }
}
