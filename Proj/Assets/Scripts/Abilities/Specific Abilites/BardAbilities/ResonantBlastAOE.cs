using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ResonantBlast_Ability", menuName = "Scriptable Objects/Abilities/Bard/Resonant Blast")]
public class ResonantBlastAOE : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.7f;
    [SerializeField] private float _damageMultiplierDebuffed = 1f;
    [SerializeField] private float _procentBaseHeal = 0.1f;
    [SerializeField] private float _procentPerExtraEnemyHit = 0.025f;
    [SerializeField] private float _maxHealProcent = 0.2f;

    private int _enemiesHit;

    // Description

    // Deal(70% � Damage) Elemental damage to all enemies in the area.Enemies with a Debuff take (100% � Damage) instead.
    // If at least one enemy is hit, heal the Bard for 10% of their maximum Health, plus 2.5% for each additional enemy hit(up to 20%).


    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, ref pattern);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        _enemiesHit = 0;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
            _enemiesHit++;
        }
        if (_enemiesHit == 0) return;

        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int healAmount = CalculateHeal(castingCharacter);

        int totalHealth = healAmount + castingCharacter.Data.CurrentHealthPoints;
        if (totalHealth >= castingCharacter.Data.DerivedHealthPoints)
        {
            healAmount = castingCharacter.Data.DerivedHealthPoints - castingCharacter.Data.CurrentHealthPoints;
        }

        if (healAmount <= 0) return;
        castingCharacter.Heal(healAmount);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, castingCharacter, targetTile, 0, healAmount, null, false);
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

    public override void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        _enemiesHit = 0;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            PreviewEffectOnTile(casterTile, tile);
            _enemiesHit++;
            var character = tile.GetOccupantCharacter();
            if (character == null) continue;
            character.ShowPreviewVFX();
            GetAbilityHandler().AddPreviewedCharacter(character);
        }
        if (_enemiesHit == 0) return;

        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int healAmount = CalculateHeal(castingCharacter);

        int totalHealth = healAmount + castingCharacter.Data.CurrentHealthPoints;
        if (totalHealth >= castingCharacter.Data.DerivedHealthPoints)
        {
            healAmount = castingCharacter.Data.DerivedHealthPoints - castingCharacter.Data.CurrentHealthPoints;
        }
        castingCharacter.ShowPreviewVFX();
        GetAbilityHandler().AddPreviewedCharacter(castingCharacter);
        if (healAmount == 0) return;
        castingCharacter.PreviewHealthChange(healAmount);
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
        // Get base damage.
        int baseDamage = castingCharacter.Data.DerivedDamage;

        bool characterHasDebuff = false;

        StatusEffectManager statusEffectManager = affectedCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return 0;
        var effects = statusEffectManager.GetAllStatusEffectsSnapshot();
        foreach (var effect in effects)
        {
            if (effect.Data.Type == StatusEffectType.Debuff)
            {
                characterHasDebuff = true;
                break;
            }
        }

        float damage = characterHasDebuff ? baseDamage * _damageMultiplierDebuffed : baseDamage * _damageMultiplier;

        damage = castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }
    private int CalculateHeal(Character castingCharacter)
    {
        int characterMaxHP = castingCharacter.Data.DerivedHealthPoints;

        float healProcent = _procentBaseHeal + (_enemiesHit * _procentPerExtraEnemyHit);
        healProcent = Mathf.Min(healProcent, _maxHealProcent);

        float healAmount = characterMaxHP * healProcent;

        healAmount = castingCharacter.GetStatusEffectManager().ModifyOutgoingHeal(healAmount, this);
        healAmount = castingCharacter.GetStatusEffectManager().ModifyIncomingHeal(healAmount, this);

        return Mathf.RoundToInt(healAmount);
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
        float damage = GetCharacterCaster().Data.DerivedDamage * _damageMultiplierDebuffed;
        damage = GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return Mathf.RoundToInt(damage);
    }
}
