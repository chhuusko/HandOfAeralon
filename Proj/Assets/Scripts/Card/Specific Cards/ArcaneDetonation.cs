using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Arcane Detonation", menuName = "Item/Card Data/Arcane Detonation", order = 1)]
public class ArcaneDetonation : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damage = 30;
    public override void PlayCard()
    {
        List<Character> characters = CombatGrid._instance.GetAllCharacterScripts();
        foreach(Character character in characters)
        {
            character.TakeDamage(GetDamage(character));
            
        }
        CardHandManager.GetInstance().AddCardFromDeck();
    }
    public override int GetDamage(Character character)
    {
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));
    }
    public override void ShowDamagePreview(Character character)
    {
        character.PreviewHealthChange(-damage);
    }
    public override void ShowDamagePreview()
    {
        List<Character> characters = CombatGrid._instance.GetAllCharacterScripts();
        foreach (Character character in characters)
        {
            ShowDamagePreview(character);
        }
    }
}
