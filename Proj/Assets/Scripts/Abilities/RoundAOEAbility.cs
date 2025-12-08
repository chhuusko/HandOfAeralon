using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class RoundAOEAbility : AOEAbility
{
    [Header("- Type Specific values - ")]
    [SerializeField] protected int _radius;

    public override IEnumerator StartAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        Character caster = casterTile.GetOccupantCharacter();
        if (caster == null) Debug.LogError("CasterTile has no character!");

        // Should not be able to move after performing ability.
        ResetMovementPoints(caster);

        // Rotate towards target if the target is not the caster's tile.
        if (casterTile != targetTile)
        {
            caster.RotateTowards(targetTile.transform, GetCastingRotationTime());
        }

        if (caster.TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger(GetAbilityName());
        }

        int aoeDelta = 0;

        if (caster.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
        {
            int baseRadius = _radius;
            int finalRadius = statusEffectManager.ApplyAoEModifiers(ref baseRadius);
            aoeDelta = finalRadius - baseRadius;
        }

        if (GetAbilityVFXSequence() != null)
        {
            VFXData data = new VFXData
            {
                Caster = caster,
                OriginPosition = casterTile.transform.position,
                TargetTile = targetTile,
                TargetPosition = targetTile.transform.position,
                Direction = (targetTile.transform.position - casterTile.transform.position).normalized,
                CastingFXDuration = GetCastingTime(),
                TravelFXDuration = GetFromCastToHitTime(),
                AoEDelta = aoeDelta
            };
            caster.StartCoroutine(GetAbilityVFXSequence().RunSequence(data)
            );
        }

        yield return new WaitForSeconds(GetCastingTime() + GetFromCastToHitTime());
        RunAbility(casterTile, targetTile);
    }

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, pattern);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                ApplyEffectOnTile(casterTile, tile);
            }
        }
    }
    public override List<CombatGridTile> GetTilesToEffect(CombatGridTile tile)
    {
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, pattern);
        }
        return _pattern.CalculateTilesToEffect(tile);
    }

    protected void SetAbilityRadius(int radius, RoundAOEPattern pattern)
    {
        int baseRadius = radius;
        if (GetCharacterCaster().TryGetComponent<StatusEffectManager>(out var statusEffectManager))
        {
            int finalRadius = statusEffectManager.ApplyAoEModifiers(ref baseRadius);
            pattern.SetRadius(finalRadius);
            return;
        }
        Debug.LogError("Could not find StatusEffectManager on object");
    }
}
