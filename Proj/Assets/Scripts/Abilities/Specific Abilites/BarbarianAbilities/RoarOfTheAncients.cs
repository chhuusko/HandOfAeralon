using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoarOfTheAncients_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Roar of the Ancients")]
public class RoarOfTheAncients : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _chanceToApplyWeakened = 0.5f;

    [SerializeField] private int _slowDuration = 2;
    [SerializeField] private int _weakenedDuration = 2;
    [SerializeField] private int _manaGain = 2;
    [SerializeField] private int _enemiesWeakenedTilBonus = 2;

    private int _enemiesWeakened;

    // Description

    // Unleash a primal roar, enemies in area have a 100% chance to become Slowed and 50% chance to become Weakened for two turns.
    // Gain 2 Mana if at least two enemies become Weakened.

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

        _enemiesWeakened = 0;
        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
        }

        // Check to see if casting character is friendly before changing mana.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (_enemiesWeakened >= _enemiesWeakenedTilBonus)
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

        StatusEffectSystem statusEffectSystem = affectedCharacter.GetComponent<StatusEffectSystem>();

        if (statusEffectSystem == null) return;

        StatusEffect slow = new Slowed(_slowDuration);

        if (Random.value < _chanceToApplyWeakened)
        {
            StatusEffect weakened = new Weakened(_weakenedDuration);
            statusEffectSystem.AddStatusEffect(weakened, castingCharacter);
            AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, weakened, false);

            _enemiesWeakened++;
        }
        statusEffectSystem.AddStatusEffect(slow, castingCharacter);
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, slow, false);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}
