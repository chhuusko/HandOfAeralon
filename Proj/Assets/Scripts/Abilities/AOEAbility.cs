using System.Collections.Generic;
using UnityEngine;

public abstract class AOEAbility : Ability
{
    [Header("- Type Specific values - ")]
    [SerializeField] protected AOEPattern _pattern;
    [SerializeField] protected ValidTargets _validTargets;
    [SerializeField] protected AbilityVFXSequence _abilityAOEVFXSequence;

    public enum ValidTargets
    {
        Any,
        Self,
        NonSelf,
        Friendlies,
        NonSelfFriendlies,
        Enemies
    }
    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
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

    public override void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                // Don't apply effect on tiles with invalid targets.
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                PreviewEffectOnTile(casterTile, targetTile);
                var character = targetTile.GetOccupantCharacter();
                if (character == null) continue;
                
            }
        }
    }

    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile targetTile)
    {
        if (targetTile == null)
            return null;

        // Get caster
        Character caster = GetAbilityHandler().GetCharacterCaster();
        if (caster == null) return null;

        // Check if target tile is in range.
        bool inRange = caster.GetAbilityHandler().GetTilesInRange().Contains(targetTile);
        if (!inRange) return null;

        return _pattern.CalculateTilesToEffect(targetTile);
    }

    /// <summary>
    /// Validates whether a tile contains a valid target for this AoE ability.
    /// Filters targets based on the selected ValidTargets setting
    /// (Any, Friendlies, Enemies) and compares factions with the caster.
    /// Empty tiles and invalid occupants are automatically rejected.
    /// </summary>
    /// <param name="casterTile">The tile of the character casting the ability.</param>
    /// <param name="targetTile">The tile being evaluated as a potential target.</param>
    /// <returns>True if the tile contains a valid target for the AoE ability.</returns>
    protected bool IsValidTargetForAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        
        if (casterTile == null || targetTile == null) return false;

        Character casterCharacter = casterTile.GetOccupantCharacter();
        Character targetCharacter = targetTile.GetOccupantCharacter();

        // Reject tiles without valid characters.
        if (casterCharacter == null || targetCharacter == null) return false;

        // Validate target based on selected targeting rule.
        switch (_validTargets)
        {
            case AOEAbility.ValidTargets.Any:
                return true;
            case AOEAbility.ValidTargets.Self:
                return casterCharacter == targetCharacter;
            case AOEAbility.ValidTargets.NonSelf:
                return casterCharacter != targetCharacter;
            case AOEAbility.ValidTargets.NonSelfFriendlies:
                return (casterCharacter != targetCharacter) && casterCharacter.GetFaction() == targetCharacter.GetFaction();
            case AOEAbility.ValidTargets.Friendlies:
                return casterCharacter.GetFaction() == targetCharacter.GetFaction();
            case AOEAbility.ValidTargets.Enemies:
                return casterCharacter.GetFaction() != targetCharacter.GetFaction();
            default: return false;
        }
    }
}

