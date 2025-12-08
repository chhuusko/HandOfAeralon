using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightningStorm_Ability", menuName = "Scriptable Objects/Abilities/Sorceress/Lightning Storm")]

public class LightningStorm_Ability : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.8f;
    [SerializeField] private float _stunCharacterHitChance = 0.25f;
    [SerializeField] private int _stunDuration = 1;
    [SerializeField] private int _stunnedEnemiesTilBonus = 1;

    [Header("- Emberwake Effects -")]
    [SerializeField] private int _burnDuration = 1;

    bool enemyStunned;

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        enemyStunned = false;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
        }

        // Check to see if casting character is friendly before drawing card.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (enemyStunned)
        {
            CardHandManager.GetInstance().AddCardFromDeck();
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
        bool died = affectedCharacter.TakeDamage(damage);

        StatusEffectManager statusEffectManager = castingCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return;

        StatusEffect stun = statusEffectManager.TryApplyStun(affectedCharacter, _stunCharacterHitChance, _stunDuration);
        StatusEffect burn = statusEffectManager.TryApplyBurn(affectedCharacter, 0, _burnDuration);

        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, stun, died);
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, burn, died);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // Get base damage.
        int baseDamage = castingCharacter.GetBaseDamage();

        int damage = (int) (baseDamage * _damageMultiplier);

        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}
