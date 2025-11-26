using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Shard Volley", menuName = "Item/Card Data/Shard Volley", order = 1)]
public class ShardVolley : Card
{
    public override void PlayCard()
    {
        int damage = 15 + (CardHandManager.GetInstance().GetCardsPlayedThisTurn()*2);
        List<GameObject> enemies = CombatGrid._instance.GetAllEnemyCharacters();
        List<Character> enemiesScript = new List<Character>();

        foreach (GameObject enemy in enemies)
        {
            enemiesScript.Add(enemy.GetComponent<Character>());
        }

        while (damage > 0)
        {
            int damageInstance = Random.Range(1, damage + 1);
            enemiesScript[Random.Range(0, enemiesScript.Count)].TakeDamage(damageInstance);
            damage -= damageInstance;
        }
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
