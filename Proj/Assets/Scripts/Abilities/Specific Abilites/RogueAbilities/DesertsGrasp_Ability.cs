using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Desert's Grasp", menuName = "Scriptable Objects/Abilities/Rogue/Desert's Grasp")]

public class DesertsGrasp_Ability : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.7f;
    [SerializeField] private float _chanceToApplyPoison = 0.7f;
    [SerializeField] private int _poisonStacks = 3;
    [SerializeField] private int _manaGain = 1;
    [SerializeField] private int _enemiesPoisonedTilBonus = 3;

    private int _enemiesPoisoned = 0;

    // Description

    // All allies in the target area gain Haste for 2 turns.
    // Gain 1 Mana if at least three allies gain Haste.

    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile targetTile)
    {
        // Works like the base version of GetTilesToEffect but only returns the list when valid target is hovered. 

        if (targetTile == null)
            return null;

        // Get caster
        Character caster = GetAbilityHandler().GetCharacterCaster();
        if (caster == null)
            return null;

        // Check if ability can be cast on target tile.
        bool canCast = caster.GetAbilityHandler().CanCastAbility(this, targetTile);
        if (!canCast) return null;

        // Calculate which tiles to effect.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        var list = _pattern.CalculateTilesToEffect(targetTile);

        return list;
    }

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        _enemiesPoisoned = 0;
        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
        }

        // Check to see if casting character is friendly before changing mana.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (_enemiesPoisoned >= _enemiesPoisonedTilBonus)
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
        bool died = affectedCharacter.TakeDamage(damage);

        StatusEffect poison = null;

        if (_abilityAOEVFXSequence != null)
        {
            VFXData data = new VFXData
            {
                Caster = GetCharacterCaster(),
                OriginPosition = casterTile.transform.position,
                TargetTile = tileToEffect,
                TargetPosition = tileToEffect.transform.position,
                Direction = (tileToEffect.transform.position - casterTile.transform.position).normalized,
            };
            data.Caster.StartCoroutine(_abilityAOEVFXSequence.RunSequence(data)
            );
        }

        if (Random.value < _chanceToApplyPoison)
        {
            if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
            {
                statusEffectManager.AddStatusEffect(poison = new Poison(castingCharacter, _poisonStacks), castingCharacter);
                _enemiesPoisoned++;
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
        float damage = castingCharacter.Data.DerivedDamage * _damageMultiplier;
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
}
