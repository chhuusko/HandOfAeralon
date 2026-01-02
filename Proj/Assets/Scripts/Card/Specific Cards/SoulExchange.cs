using UnityEngine;

[CreateAssetMenu(fileName = "Soul Exchange", menuName = "Item/Card Data/Soul Exchange", order = 1)]
public class SoulExchange : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCardOnTarget(Character character)
    {
        Character activeCharacter = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        character.TakeDamage(999);
        activeCharacter.TakeDamage(999);
    }
}
