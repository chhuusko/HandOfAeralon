using UnityEngine;

[CreateAssetMenu(fileName = "Frenzy Injection", menuName = "Item/Card Data/Frenzy Injection", order = 1)]
public class FrenzyInjection : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {

        if (character != null)
        {
            character.TakeDamage(3);
            character.GetStatusEffectManager().AddStatusEffect(new Enraged(2));
        }
    }

    // Update is called once per frame
   
}
