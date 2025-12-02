using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Earthquake_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Earthquake_Ability")]

public class Earthquake_AOE : AOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.9f;
    [SerializeField] private float _slowCharacterHitChance = 0.6f;
    [SerializeField] private int _slowDuration = 2;
    [SerializeField] private int _charactersSlowedToGetMana = 2;
    [SerializeField] private int _manaGain = 1;

    // Description

    // Slam the ground, dealing(90% × Damage) Physical damage to all characters in the area.
    // Every character hit has a 60% chance to become Slowed for 2 turns.
    // Gain 1 Mana if at least two enemies become Slowed.



    private int slowedEnemyCounter;

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        slowedEnemyCounter = 0;
        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                ApplyEffectOnTile(casterTile, tile);
            }
        }
        if (slowedEnemyCounter >= _charactersSlowedToGetMana)
        {
            CardHandManager.GetInstance().ChangeMana(_manaGain);
        }
    }

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        affectedCharacter.TakeDamage(damage);
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0);

        if (Random.value < _slowCharacterHitChance)
        {
            if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
            {
                statusEffectManager.AddStatusEffect(new Slowed(_slowDuration));
                if (castingCharacter.GetFaction() == Faction.Friendly && affectedCharacter.GetFaction() == Faction.Enemy)
                {
                    slowedEnemyCounter++;
                }
            }
        }
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        int damage = (int)(castingCharacter.GetBaseDamage() * _damageMultiplier);
        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}
