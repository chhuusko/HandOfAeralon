using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RuptureOfTheWilds_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Rupture of the Wilds")]

public class RuptureOfTheWildsAOE : DirectedAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1f;
    [SerializeField] private float _slowedTargetDamageMultiplier = 1.5f;
    [SerializeField] private float _stunCharacterHitChance = 0.3f;
    [SerializeField] private int _stunDuration = 2;

    // Description

    // Send primal energy through the ground, dealing (100 % � Damage) Elemental damage.
    // If the target is Slowed, deal (150% � Damage) instead.
    // Every character hit has a 30% chance to become Stunned for 2 turns.
    // Draw 1 card per enemy Stunned by this ability.

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

        StatusEffect stun = statusEffectManager.TryApplyStun(affectedCharacter, _stunCharacterHitChance, _stunDuration);

        if (stun != null && castingCharacter.GetFaction() == Faction.Friendly && affectedCharacter.GetFaction() == Faction.Enemy)
        {
            CardHandManager.GetInstance().AddCardFromDeck();
        }

        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, stun, died);
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
        // Get base damage.
        int baseDamage = castingCharacter.Data.DerivedDamage;
        var statusEffectsManager = affectedCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectsManager == null) return 0;

        // If character is slowed, deal more damage.
        bool targetIsSlowed = statusEffectsManager.GetStatusEffect<Slowed>() != null;

        float damage = targetIsSlowed ? (baseDamage * _slowedTargetDamageMultiplier) : (baseDamage * _damageMultiplier);

        damage = castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }

    public override int GetDamage()
    {
        float damage = GetCharacterCaster().Data.DerivedDamage * _damageMultiplier;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }

    public override int GetSecondDamage()
    {
        float damage = GetCharacterCaster().Data.DerivedDamage * _slowedTargetDamageMultiplier;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }
}
