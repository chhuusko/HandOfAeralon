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

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        int damage = (int)(castingCharacter.GetBaseDamage() * _damageMultiplier);
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
