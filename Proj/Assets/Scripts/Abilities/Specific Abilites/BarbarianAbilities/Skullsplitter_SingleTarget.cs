using UnityEngine;

[CreateAssetMenu(fileName = "Skullsplitter_Ability", menuName = "Scriptable Objects/Abilities/Range Calculations/Non-Blocking")]
public class Skullsplitter_Ability : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private int _baseDamage;
    [SerializeField] private float damageMultiplier = 1.6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void ApplyEffectOnTile(CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;

        affectedCharacter.TakeDamage(CalculateDamage(affectedCharacter));
    }

    private int CalculateDamage(Character affectedCharacter)
    {
        // Calculate increasedDamage;
        int increasedDamage = (int)(_baseDamage * damageMultiplier);

        // Decide which damage to use.
        int calculatedDamge = affectedCharacter.GetCurrentHealth() < (0.5 * affectedCharacter.GetMaxHealth()) ? increasedDamage : _baseDamage;

        return 0;
    }

}
