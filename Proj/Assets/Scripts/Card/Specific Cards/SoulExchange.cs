using UnityEngine;

[CreateAssetMenu(fileName = "Soul Exchange", menuName = "Item/Card Data/Soul Exchange", order = 1)]
public class SoulExchange : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        Character activeCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        character.TakeDamage(GetDamage(character));
        activeCharacter.TakeDamage(GetDamage(activeCharacter));
    }
    public override int GetDamage(Character character)
    {
        int damage = 9999;
        return damage;
    }
    public override void ShowDamagePreview(Character character)
    {
        Character activeCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        activeCharacter.PreviewHealthChange(-GetDamage(activeCharacter));
        character.PreviewHealthChange(-GetDamage(character));
    }
}
