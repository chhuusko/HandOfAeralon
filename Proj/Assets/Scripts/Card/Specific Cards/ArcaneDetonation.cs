using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arcane Detonation", menuName = "Item/Card Data/Arcane Detonation", order = 1)]
public class ArcaneDetonation : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {
        List<Character> characters = CombatGrid._instance.GetAllCharacterScripts();
        foreach(Character c in characters)
        {
            c.TakeDamage(6);
        }
        CardHandManager.GetInstance().AddCardFromDeck();
    }
}
