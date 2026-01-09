using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sundering Wave", menuName = "Item/Card Data/Sundering Wave", order = 1)]
public class SunderingWave : Card
{
    public override void PlayCard()
    {
        List<GameObject> enemies = CombatGrid._instance.GetAllEnemyCharacters();
        foreach(GameObject enemy in enemies)
        {
            Character character = enemy.GetComponent<Character>();
            if (character.GetStatusEffectManager().ContainsStatusEffect<Slowed>())
            {
                int damage = 60;
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
                character.TakeDamage(damage);
            } else 
            {
                int damage = 40;
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
                character.TakeDamage(damage);
            }
            
        }
    }
    public override void ShowDamagePreview()
    {
        List<GameObject> enemies = CombatGrid._instance.GetAllEnemyCharacters();
        foreach (GameObject enemy in enemies)
        {
            Character character = enemy.GetComponent<Character>();
            if (character.GetStatusEffectManager().ContainsStatusEffect<Slowed>())
            {
                int damage = 60;
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
                character.PreviewHealthChange(-damage);
            }
            else
            {
                int damage = 40;
                damage = Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
                character.PreviewHealthChange(-damage);
            }

        }
    }
}
