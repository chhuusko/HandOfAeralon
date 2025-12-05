using UnityEngine;

[CreateAssetMenu(fileName = "ArcaneBolt_Ability", menuName = "Scriptable Objects/Abilities/Sorceress/Arcane Bolt")]
public class ArcaneBolt_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.4f;
    [SerializeField] private float _manaDamageMultiplier = 0.1f;
    [SerializeField] private int _enemyManaAmount = 6;

    [Header("- Emberwake Effects -")]
    [SerializeField] private float _emberwakeBurnChance = 0.25f;
    [SerializeField] private int _emberwakeBurnAmount = 2;


    // Description

    // Deals((40% + 10% per current Mana) × Damage) Elemental damage. (Enemy always has 6 Mana)

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        bool died = affectedCharacter.TakeDamage(damage);

        StatusEffectManager statusEffectManager = castingCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return;

        StatusEffect burn = castingCharacter.TryApplyBurn(affectedCharacter, _emberwakeBurnChance, _emberwakeBurnAmount);

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, burn, died);
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

        int baseDamage = castingCharacter.GetBaseDamage();
        int mana = castingCharacter.GetFaction() == Faction.Friendly ? CardHandManager.GetInstance().GetMana() : _enemyManaAmount;

        float totalMultiplier = _damageMultiplier + (_manaDamageMultiplier * mana);
        int damage = (int)(baseDamage * totalMultiplier);


        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);

        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}