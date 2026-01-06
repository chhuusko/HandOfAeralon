using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DissonantChord_Ability", menuName = "Scriptable Objects/Abilities/Bard/Dissonant Chord")]

public class DissonantChordAOE : RoundAOEAbility
{
    [Header("- Ability Specific values -")]

    [SerializeField] private int _enemiesDebuffedTilBonus = 2;
    [SerializeField] private float _damageMultiplier = 0.9f;
    [SerializeField] private float _damagePerBuffMultiplier = 0.1f;
    [SerializeField] private float _maxDamageMultiplier = 1.4f;


    // Description

    // Remove all buffs from enemies in the area.
    // Deal((90% + (10% � amount of buffs removed) (up to 140%) � Damage) elemental damage.
    // Draw 1 card if at least two buffs are removed from enemies.


    int enemiesDebuffed;

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, ref pattern);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        enemiesDebuffed = 0;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            RemoveBuffFromEnemy(casterTile, tile);
        }

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
        }

        // Check to see if casting character is friendly before drawing card.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (enemiesDebuffed >= _enemiesDebuffedTilBonus)
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

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, null, died);
    }
    private void RemoveBuffFromEnemy(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
        {
            int buffsCleared = statusEffectManager.ClearStatusEffects(StatusEffectType.Buff);

            if (castingCharacter.GetFaction() != affectedCharacter.GetFaction())
            {
                enemiesDebuffed += buffsCleared;
            }
        }
    }
    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // Get base damage.
        int baseDamage = castingCharacter.Data.DerivedDamage;

        float damageMultiplier = Mathf.Min(_damageMultiplier + (_damagePerBuffMultiplier * enemiesDebuffed), _maxDamageMultiplier);

        float damage = baseDamage * damageMultiplier;

        damage = castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }

   

    public override void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, ref pattern);
        }

        // Calculate all tiles around with in radius and apply effect to all of them.
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        enemiesDebuffed = 0;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            RemoveBuffFromEnemy(casterTile, tile);
        }

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                PreviewEffectOnTile(casterTile, tile);
                var character = tile.GetOccupantCharacter();
                if (character == null) continue;
                character.ShowPreviewVFX();
                GetAbilityHandler().AddPreviewedCharacter(character);
            }
        }
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

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Not implemented.
    }

    public override int GetDamage()
    {
        float damage = GetCharacterCaster().Data.DerivedDamage * _damageMultiplier;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }

    public override int GetSecondDamage()
    {
        float damage = GetCharacterCaster().Data.DerivedDamage * _maxDamageMultiplier;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }
}
