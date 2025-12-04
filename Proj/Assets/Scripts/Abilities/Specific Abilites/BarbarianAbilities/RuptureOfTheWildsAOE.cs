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
        affectedCharacter.TakeDamage(damage);

        StatusEffect stun = null;

        if (Random.value < _stunCharacterHitChance)
        {
            if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
            {
                statusEffectManager.AddStatusEffect(new Stunned(_stunDuration), castingCharacter);
                if (castingCharacter.GetFaction() == Faction.Friendly && affectedCharacter.GetFaction() == Faction.Enemy)
                {
                    CardHandManager.GetInstance().AddCardFromDeck();
                }
            }
        }
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, stun);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // Get base damage.
        int baseDamage = castingCharacter.GetBaseDamage();
        var statusEffectsManager = affectedCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectsManager == null) return 0;

        // If character is slowed, deal more damage.
        bool targetIsSlowed = statusEffectsManager.GetStatusEffect<Slowed>() != null;

        int damage = targetIsSlowed ? (int)(baseDamage * _damageMultiplier) : (int)(baseDamage * _slowedTargetDamageMultiplier);

        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}
