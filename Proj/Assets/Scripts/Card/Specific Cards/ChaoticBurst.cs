using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(fileName = "Chaotic Burst", menuName = "Item/Card Data/Chaotic Burst", order = 1)]
public class ChaoticBurst : Card
{
    [SerializeField] private int totalDamage = 75;
    public override void PlayCard()
    {

        List<Character> characters = CombatGrid._instance.GetAllCharacterScripts();
        while (totalDamage > 0) 
        {
            int damageInstance = Random.Range(1, totalDamage + 1);
            int damage = damageInstance;
            Character characterTarget = characters[Random.Range(0, characters.Count)];

            damage = Mathf.RoundToInt(characterTarget.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
            characterTarget.TakeDamage(damage);

            totalDamage -= damageInstance;

            
        }
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
