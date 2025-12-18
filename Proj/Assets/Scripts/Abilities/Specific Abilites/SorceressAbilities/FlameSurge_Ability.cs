using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlameSurge_Ability", menuName = "Scriptable Objects/Abilities/Sorceress/Flame Surge")]
public class FlameSurge_Ability : DirectedAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1f;
    [SerializeField] private float _chanceToBurnCharacters = 0.5f;
    [SerializeField] private int _burnDuration = 2;
    [SerializeField] private int _charactersBurnedToGainMana = 2;
    [SerializeField] private int _manaGain = 1;

    [Header("- Emberwake Effects -")]
    [SerializeField] private int _emberwakeBurnAmount = 2;



    // Description

    // Unleash a burst of fire, dealing(100% � Damage) Elemental damage to all characters in area.
    // Every character hit has a 50% chance to gain Burn for 2 turns.
    // Gain 1 Mana if at least two enemies become Burned.



    private int burnedEnemiesCounter;

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

        burnedEnemiesCounter = 0;
        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                ApplyEffectOnTile(casterTile, tile);
            }
        }

        // Check to see if casting character is friendly before changing mana.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (burnedEnemiesCounter >= _charactersBurnedToGainMana)
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


        StatusEffectManager statusEffectManager = castingCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return;

        int burnDuration = Mathf.Max(_emberwakeBurnAmount, _burnDuration);
        StatusEffect burn = statusEffectManager.TryApplyBurn(affectedCharacter, _chanceToBurnCharacters, burnDuration);

        if (burn != null)
        {
            burnedEnemiesCounter++;
        }

        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, burn, died);
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
        int damage = Mathf.RoundToInt(castingCharacter.Data.DerivedDamage * _damageMultiplier);
        damage = Mathf.RoundToInt(castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this));
        damage = Mathf.RoundToInt(affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this));
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }

    public override int GetDamage()
    {
        int damage = Mathf.RoundToInt(GetCharacterCaster().Data.DerivedDamage * _damageMultiplier);
        damage = Mathf.RoundToInt(GetCharacterCaster().GetStatusEffectManager().ModifyOutgoingDamage(damage, this));
        return damage;
    }
}
