using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Chaotic Burst", menuName = "Item/Card Data/Chaotic Burst", order = 1)]
public class ChaoticBurst : Card
{
    public override void PlayCard()
    {

        int damage = 15;
        List<Character> characters = CombatGrid._instance.GetAllCharacterScripts();
        while (damage > 0) 
        {
            int damageInstance = Random.Range(1, damage + 1);
            characters[Random.Range(0, characters.Count)].TakeDamage(damageInstance);
            damage -= damageInstance;
        }
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
