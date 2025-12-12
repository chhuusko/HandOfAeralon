using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowingKnives_Ability", menuName = "Scriptable Objects/Abilities/Rogue/Throwing Knives")]
public class ThrowingKnives_AOE : DirectedAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.6f;
    [SerializeField] private float _chanceToApplyPoison = 0.6f;
    [SerializeField] private float _handSizeDamageMultiplier = 0.1f;
    [SerializeField] private int _poisonStacks = 3;

    // Description

    // Throw knives in a line, dealing (60 % +(10 % � current hand size) � Damage) Physical damage.
    // Every character hit has a 60% chance to gain 3 stacks of Poison.

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

        if (Random.value < _chanceToApplyPoison)
        {
            if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
            {
                statusEffectManager.AddStatusEffect(poison = new Poison(_poisonStacks), castingCharacter);

            }
        }
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, poison, died);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        int baseDamage = castingCharacter.GetBaseDamage();
        int cardsAmount = CardHandManager.GetInstance().GetCardsInHand().Count;

        int damage = (int)(baseDamage * _damageMultiplier);
        damage += (int)(baseDamage * _handSizeDamageMultiplier * cardsAmount);
        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }
    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }

    public override int GetDamage()
    {
        int damage = (int)(GetCharacterCaster().GetBaseDamage() * _damageMultiplier);
        damage = (int)GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return damage;
    }
}
