using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LuteSmash_Ability", menuName = "Scriptable Objects/Abilities/Bard/Lute Smash")]
public class LuteSmash_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1.7f;
    [SerializeField] private float _applyStunChance = 0.35f;
    [SerializeField] private int _stunDuration = 1;
    [SerializeField] private int _manaGain = 2;

    [Header("- Available Abilities after LuteSmash -")]
    [SerializeField] List<Ability> abilitiesAvailablePostLuteSmash;

    // Description

    // Deal(170% � Damage) Physical damage.
    // Has a 35% chance to Stun the target for 1 turn.
    // Gain 2 Mana if the target gets Stunned by this ability.
    // After using Lute Smash, only Lute Smash and Dissonant Chord can be used for the rest of combat.


    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        bool died = affectedCharacter.TakeDamage(damage);

        castingCharacter.Data.SetActiveAbilities(abilitiesAvailablePostLuteSmash); 

        StatusEffectManager statusEffectManager = castingCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return;

        StatusEffect stun = statusEffectManager.TryApplyStun(affectedCharacter, _applyStunChance, _stunDuration);

        if (stun != null && castingCharacter.GetFaction() == Faction.Friendly)
        {
            CardHandManager.GetInstance().ChangeMana(_manaGain);
        }

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, stun, died);
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
        int damage = castingCharacter.Data.DerivedDamage;
        damage = Mathf.RoundToInt(damage * _damageMultiplier);


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
        int damage = (int)(GetCharacterCaster().Data.DerivedDamage * _damageMultiplier);
        damage = (int)GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return damage;
    }
}
