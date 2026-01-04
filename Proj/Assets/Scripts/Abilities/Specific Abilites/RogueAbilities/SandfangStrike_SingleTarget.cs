using UnityEngine;


[CreateAssetMenu(fileName = "SandfangStrike_Ability", menuName = "Scriptable Objects/Abilities/Rogue/Sandfang Strike")]
public class SandfangStrike_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1.3f;
    [SerializeField] private float _applyPoisonChance = 0.8f;
    [SerializeField] private int _posionStacksToApply = 3;

    // Description

    // Deal(130% × Damage) Physical damage.
    // Has an 80% chance to apply 3 stacks of Poison to the target.
    // If the target already had Poison, draw 1 card.

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        bool died = affectedCharacter.TakeDamage(damage);

        StatusEffect poison = null;
        if(Random.value <= _applyPoisonChance && affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager)){
            if (castingCharacter.GetFaction() == Faction.Friendly && statusEffectManager.ContainsStatusEffect<Poison>()){
                CardHandManager.GetInstance().AddCardFromDeck();
            }
            statusEffectManager.AddStatusEffect(poison = new Poison(_posionStacksToApply, castingCharacter), castingCharacter);
        }
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, poison, died);
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

     
        float damage = castingCharacter.Data.DerivedDamage;
        damage = damage * _damageMultiplier;


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
}
