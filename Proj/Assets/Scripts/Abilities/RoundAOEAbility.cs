using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class RoundAOEAbility : AOEAbility
{
    [Header("- Type Specific values - ")]
    [SerializeField] protected int _radius;

    public int GetRadius() => _radius;

    public override IEnumerator StartAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        Character caster = casterTile.GetOccupantCharacter();
        if (caster == null) Debug.LogError("CasterTile has no character!");

        caster.Animator.SetBool("AbilityOngoing", true);

        // Should not be able to move after performing ability.
        caster.CanMove = false;

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

        AudioManager.Instance.PlayOneShot(AudioEvent, caster.transform.position);

        if (GetAbilityVFXSequence() != null)
        {
            VFXData data = new VFXData
            {
                Caster = caster,
                OriginPosition = casterTile.transform.position,
                TargetTile = targetTile,
                TargetPosition = targetTile.transform.position,
                Direction = (targetTile.transform.position - casterTile.transform.position).normalized,
                CastingAnimationDuration = GetCastingAnimationTime(),
                CastingFXDuration = GetCastingTime(),
                TravelFXDuration = GetFromCastToHitTime(),
                ImpactFXDuration = GetImpactTime(),
                AoEDelta = aoeDelta
            };
            yield return caster.StartCoroutine(GetAbilityVFXSequence().RunSequence(data)
            );
        }

        RunAbility(casterTile, targetTile);
        Selector._instance.InvokeCharacterActionStopped();
        CombatEventManager.InvokeAfterAbilityCast(caster, this);
        yield return new WaitForSeconds(3);

        caster.Animator.SetBool("AbilityOngoing", false);
    }

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, ref pattern);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
        }
    }

    public override void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, ref pattern);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                if (!IsValidTargetForAbility(casterTile, tile)) continue;

                PreviewEffectOnTile(casterTile, tile);
                var character = tile.GetOccupantCharacter();
                if (character == null) continue;
                character.ShowPreviewVFX();
                GetAbilityHandler().AddPreviewedCharacter(character);
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

        if (_pattern is RoundAOEPattern pattern)
        {
            SetAbilityRadius(_radius, ref pattern);
        }
        return _pattern.CalculateTilesToEffect(targetTile);
    }

    protected void SetAbilityRadius(int radius, ref RoundAOEPattern pattern)
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
