using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Apply Card Effect On Faction Member", menuName = "Item/Card Turn Effect Data/Apply Card Effect On Faction Member", order = 1)]
public class ApplyCardEffectOnFactionMember : TurnEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Faction affectedFaction;
    protected override void Effect(Character character, Card card)
    {
        
        if (character.GetFaction() == affectedFaction)
        {
            CombatGrid._instance.GetCharacterScriptsByFaction(character.GetFaction());        
        }
        
    }
}
