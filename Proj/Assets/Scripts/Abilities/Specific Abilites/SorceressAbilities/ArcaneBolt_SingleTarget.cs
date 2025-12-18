using UnityEngine;

[CreateAssetMenu(fileName = "ArcaneBolt_Ability", menuName = "Scriptable Objects/Abilities/Sorceress/Arcane Bolt")]
public class ArcaneBolt_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.4f;
    [SerializeField] private float _manaDamageMultiplier = 0.1f;

    [Header("- Emberwake Effects -")]
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

        StatusEffect burn = statusEffectManager.TryApplyBurn(affectedCharacter, 0, _emberwakeBurnAmount);

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, burn, died);
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
        // 1. Your Base Damage(Can also be applied by traits or cards.)
        // 2. Ability damage.
        // 3. Your Traits
        // 4. Your Buffs/ Debuffs(Can also be applied by cards.)
        // 5. Eventuella Ability Global Modifiers(Ex.Arena modifiers)
        // 6. EnemyTraits
        // 7. Enemy Buffs / Debuffs

        int baseDamage = castingCharacter.Data.DerivedDamage;
        int mana = castingCharacter.GetFaction() == Faction.Friendly ? CardHandManager.GetInstance().GetMana() : CombatManager._instance.enemyMana;

        float totalMultiplier = _damageMultiplier + (_manaDamageMultiplier * mana);
        int damage = Mathf.RoundToInt(baseDamage * totalMultiplier);


        damage = Mathf.RoundToInt(castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this));
        damage = Mathf.RoundToInt(affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this));

        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }

    public override int GetDamage()
    {
        int baseDamage = GetCharacterCaster().Data.DerivedDamage;
        int mana = CardHandManager.GetInstance().GetMana();

        float totalMultiplier = _damageMultiplier + (_manaDamageMultiplier * mana);
        int damage = Mathf.RoundToInt(baseDamage * totalMultiplier);
        damage = Mathf.RoundToInt(GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this));
        return damage;
    }
}