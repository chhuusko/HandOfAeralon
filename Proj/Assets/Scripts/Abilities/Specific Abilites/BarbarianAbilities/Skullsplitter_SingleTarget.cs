using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(fileName = "Skullsplitter_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Skullsplitter")]
public class Skullsplitter_Ability : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1f;
    [SerializeField] private float _extraDamageMultiplier = 1.6f;

    // Description

    // Deal(100% × Damage) Physical damage.
    // If the target is below 50% Health, deal (160% × Damage) damage instead and draw 1 card.

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;
        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        bool died = affectedCharacter.TakeDamage(damage);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, null, died);
    }

    protected override void PreviewEffectOnTile(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        if (targetTile == null) return;

        Character affectedCharacter = targetTile.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        affectedCharacter.PreviewHealthChange(-damage);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // 1. Your Base Damage(Kan �kas med traits och eller kort.)
        // 2. Ability damage.
        // 3. Your Traits
        // 4. Your Buffs/ Debuffs(Kan �ven appliceras av kort)
        // 5. Eventuella Ability Global Modifiers(Ex.Arena modifiers)
        // 6. EnemyTraits
        // 7. Enemy Buffs / Debuffs

        //1.
        int baseDamage = castingCharacter.Data.DerivedDamage;

        //2.
        float damage = affectedCharacter.GetCurrentHealth() < (0.5 * affectedCharacter.Data.DerivedHealthPoints) ? baseDamage * _extraDamageMultiplier : baseDamage * _damageMultiplier;

        damage = castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return Mathf.RoundToInt(damage);

    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }

    public override int GetDamage()
    {
        float damage = GetCharacterCaster().Data.DerivedDamage * _damageMultiplier;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }

    public override int GetSecondDamage()
    {
        float damage = GetCharacterCaster().Data.DerivedDamage * _extraDamageMultiplier;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }
}
