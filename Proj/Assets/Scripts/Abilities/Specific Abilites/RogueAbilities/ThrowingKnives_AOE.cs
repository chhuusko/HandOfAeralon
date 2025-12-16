using System.Collections;
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

        List<CombatGridTile> tilesToEffect = directedAOEPattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                Character castingCharacter = casterTile.GetOccupantCharacter();
                VFXData data;
                if (_abilityAOEVFXSequence == null) continue;
                
                    data = new VFXData
                    {
                        Caster = castingCharacter,
                        OriginPosition = castingCharacter.transform.position,
                        TargetTile = tile,
                        TargetPosition = tile.transform.position,
                        Direction = (tile.transform.position - casterTile.transform.position).normalized,
                    };
                
                // Don't apply effect on tiles with invalid targets.
                // if target is not valid, start effect on the ground instead of player;
                if (!IsValidTargetForAbility(casterTile, tile))
                {
                    data.TargetPosition += new Vector3(0, -_abilityAOEVFXSequence.GetImpactAirDistance(), 0);
                    castingCharacter.StartCoroutine(_abilityAOEVFXSequence.RunSequence(data));
                    continue;
                }

                castingCharacter.StartCoroutine(_abilityAOEVFXSequence.RunSequence(data));
                ApplyEffectOnTile(casterTile, tile);
            }
        }
    }

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
          
        if (tileToEffect == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;


        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;

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
        int baseDamage = castingCharacter.Data.DerivedDamage;
        int cardsAmount = castingCharacter.GetFaction() == Faction.Friendly? CardHandManager.GetInstance().GetCardsInHand().Count: 5;

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
        int baseDamage = GetCharacterCaster().Data.DerivedDamage;
        int cardsAmount = CardHandManager.GetInstance().GetCardsInHand().Count;

        int damage = (int)(baseDamage * _damageMultiplier);
        damage += (int)(baseDamage * _handSizeDamageMultiplier * cardsAmount);
        damage = (int)GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return damage;
    }
}
