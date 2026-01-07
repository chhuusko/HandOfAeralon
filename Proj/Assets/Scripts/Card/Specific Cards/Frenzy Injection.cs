using UnityEngine;

[CreateAssetMenu(fileName = "Frenzy Injection", menuName = "Item/Card Data/Frenzy Injection", order = 1)]
public class FrenzyInjection : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            
            character.TakeDamage(GetDamage(character));
            character.GetStatusEffectManager().AddStatusEffect(new Enraged(2));
        }
    }

    public override int GetDamage(Character character)
    {
        int damage = 25;
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
        
    }
   
}
