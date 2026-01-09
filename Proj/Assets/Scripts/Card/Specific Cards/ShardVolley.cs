using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(fileName = "Shard Volley", menuName = "Item/Card Data/Shard Volley", order = 1)]
public class ShardVolley : Card
{
    public override void PlayCard()
    {
        int damageTotal = 70 + (CardHandManager.GetInstance().GetCardsPlayedThisTurn()*10);
        List<Character> enemies = new List<Character>(CombatGrid._instance.GetCharacterScriptsByFaction(Faction.Enemy));
        
        while (damageTotal > 0)
        {
            if (enemies.Count == 0) return;
            int damageInstance = Random.Range(1, damageTotal + 1);
            Character enemyTarget = enemies[Random.Range(0, enemies.Count)];
            
            

            if (enemyTarget.GetCurrentHealth() <= 0)
            {
                enemies.Remove(enemyTarget);
            }
            else
            {
                int damage = Mathf.RoundToInt(enemyTarget.GetStatusEffectManager().ModifyIncomingDamage(damageInstance, null));

                damageTotal -= Mathf.Min(enemyTarget.GetCurrentHealth(), damageInstance);
                enemyTarget.TakeDamage(Mathf.Min(enemyTarget.GetCurrentHealth(), damage));
                
            }
        }
    }
    public override void AfterCardPlay()
    {
        base.AfterCardPlay();
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
