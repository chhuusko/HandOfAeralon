using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(fileName = "Shard Volley", menuName = "Item/Card Data/Shard Volley", order = 1)]
public class ShardVolley : Card
{
    public override void PlayCard()
    {
        int damageTotal = 15 + (CardHandManager.GetInstance().GetCardsPlayedThisTurn()*2);

        
        

        List<Character> enemies = CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Enemy);
        
        while (damageTotal > 0)
        {
            int damageInstance = Random.Range(1, damageTotal + 1);

            Character character = enemies[Random.Range(0, enemies.Count)];
            int damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyOutgoingDamage(damageInstance, null));

            character.TakeDamage(damage);


            damageTotal -= damageInstance;
        }
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
