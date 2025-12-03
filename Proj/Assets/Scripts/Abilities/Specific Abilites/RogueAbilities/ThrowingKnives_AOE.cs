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

    // Throw knives in a line, dealing (60 % +(10 % × current hand size) × Damage) Physical damage.
    // Every character hit has a 60% chance to gain 3 stacks of Poison.

    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile targetTile)
    {
        // Works like the base version of GetTilesToEffect but only returns the list when valid target is hovered. 
        // Also removes caster tile as target. 

        if (targetTile == null)
            return null;

        // Get caster
        Character caster = GetAbilityHandler().GetCharacterCaster();
        if (caster == null) return null;

        // Check if ability can be cast on target tile.
        bool canCast = caster.GetAbilityHandler().CanCastAbility(this, targetTile);
        if (!canCast) return null;

        var tile = caster.GetCurrentTileComponent();

        if (tile == null) return null;

        var directedAOEPattern = _pattern as DirectedAOEPattern;

        if (directedAOEPattern == null)
        {
            Debug.LogError("Pattern is not a DirectedAOEPattern");
            return null;
        }

        directedAOEPattern.SetDirection(CalculateDirection(tile, targetTile));

        // Calculate which tiles to effect.
        var list = _pattern.CalculateTilesToEffect(targetTile);

        // Remove caster tile. Unnecessary if pattern already removes caster.
        if (caster.GetCurrentTileComponent())
        {
            list.Remove(caster.GetCurrentTileComponent());
        }

        return list;
    }

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around within pattern and apply effect to all of them.

        var directedAOEPattern = _pattern as DirectedAOEPattern;

        if (directedAOEPattern == null)
        {
            Debug.LogError("Pattern is not a DirectedAOEPattern");
            return;
        }

        directedAOEPattern.SetDirection(CalculateDirection(casterTile, targetTile));
        directedAOEPattern.SetCasterTile(casterTile);

        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);


        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                ApplyEffectOnTile(casterTile, tile);
            }
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

        StatusEffect poison = null;

        if (Random.value < _chanceToApplyPoison)
        {
            if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
            {
                statusEffectManager.AddStatusEffect(poison = new Poison(_poisonStacks));

            }
        }
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, poison);
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
}
