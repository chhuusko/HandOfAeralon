using UnityEngine;
[CreateAssetMenu(fileName = "Chaotic Burst", menuName = "Item/Card Data/Chaotic Burst", order = 1)]
public class ChaoticBurst : Card
{
    public override void PlayCard()
    {
<<<<<<< Updated upstream
        //när den spelas
=======

        int damage = 15;
        List<Character> character = CombatGrid._instance.GetAllCharacterScripts();
        Debug.Log(character.Count);
        if (character.Count <= 0) return;
        while (damage > 0) 
        {

            int damageInstance = Random.Range(1, damage + 1);
            character[Random.Range(0, character.Count)].TakeDamage(damageInstance);
            damage -= damageInstance;
            
        }
>>>>>>> Stashed changes
    }
}
