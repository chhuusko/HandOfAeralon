using UnityEngine;

[CreateAssetMenu(fileName = "LuteSmash_Ability", menuName = "Scriptable Objects/Abilities/Bard/Lute Smash")]
public class LuteSmash_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1.7f;
    [SerializeField] private float _applyStunChance = 0.35f;
    [SerializeField] private int _stunDuration = 1;
    [SerializeField] private int _manaGain = 2;

    // Description

    // Deal(170% × Damage) Physical damage.
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

        // TODO:
        // remove the ability to use Song of Renewal and Inspiring Anthem

        StatusEffectManager statusEffectManager = castingCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return;

        StatusEffect stun = statusEffectManager.TryApplyStun(affectedCharacter, 0, _stunDuration);

        if (stun != null && castingCharacter.GetFaction() == Faction.Friendly)
        {
            CardHandManager.GetInstance().ChangeMana(_manaGain);
        }

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, stun, died);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        int damage = castingCharacter.GetBaseDamage();
        damage = (int)(damage * _damageMultiplier);


        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}
