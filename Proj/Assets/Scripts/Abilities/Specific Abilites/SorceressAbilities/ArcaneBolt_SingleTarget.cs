using UnityEngine;

[CreateAssetMenu(fileName = "ArcaneBolt_Ability", menuName = "Scriptable Objects/Abilities/Sorceress/Arcane Bolt")]
public class ArcaneBolt_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.4f;
    [SerializeField] private float _manaDamageMultiplier = 0.1f;
    [SerializeField] private int _enemyManaAmount = 6;



    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        affectedCharacter.TakeDamage(damage);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // 1. Your Base Damage(Can also be applied by traits or cards.)
        // 2. Ability damage.
        // 3. Your Traits
        // 4. Your Buffs/ Debuffs(Can also be applied by cards.)
        // 5. Eventuella Ability Global Modifiers(Ex.Arena modifiers)
        // 6. EnemyTraits
        // 7. Enemy Buffs / Debuffs

        int damage = castingCharacter.GetBaseDamage();
        damage = (int)(damage * _damageMultiplier);

        if (castingCharacter.GetFaction() == Faction.Friendly)
        {
            damage += (int)(_manaDamageMultiplier * CardHandManager.GetInstance().GetMana());
        }
        else
        {
            damage += (int)(_manaDamageMultiplier * _enemyManaAmount);
        }
        
        damage = (int) castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int) affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);

        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}