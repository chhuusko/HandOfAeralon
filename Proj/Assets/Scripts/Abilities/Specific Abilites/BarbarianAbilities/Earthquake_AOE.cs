using Newtonsoft.Json.Bson;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Earthquake_Ability", menuName = "Scriptable Objects/Abilities/Barbarian/Earthquake_Ability")]

public class Earthquake_AOE : DirectedAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 0.9f;
    [SerializeField] private float _slowCharacterHitChance = 0.6f;
    [SerializeField] private int _slowDuration = 2;
    [SerializeField] private int _charactersSlowedToGetMana = 2;
    [SerializeField] private int _manaGain = 1;

    // Description

    // Slam the ground, dealing(90% � Damage) Physical damage to all characters in the area.
    // Every character hit has a 60% chance to become Slowed for 2 turns.
    // Gain 1 Mana if at least two enemies become Slowed.



    private int slowedEnemyCounter;


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

        AudioManager.Instance.PlayOneShot(AudioEvent, caster.transform.position);

        PlayCustomEarthquakeVFX(casterTile, targetTile);

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
                ImpactFXDuration = GetImpactTime()
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

        slowedEnemyCounter = 0;
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

        if (slowedEnemyCounter >= _charactersSlowedToGetMana)
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


        StatusEffect slow = null;

        if (Random.value < _slowCharacterHitChance)
        {
            if (affectedCharacter.TryGetComponent<StatusEffectSystem>(out var statusEffectManager))
            {
                statusEffectManager.AddStatusEffect(slow = new Slowed(_slowDuration), castingCharacter);
                if (castingCharacter.GetFaction() == Faction.Friendly && affectedCharacter.GetFaction() == Faction.Enemy)
                {
                    slowedEnemyCounter++;
                }
            }
        }
        AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0, slow, died);
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        float damage = castingCharacter.Data.DerivedDamage * _damageMultiplier;
        damage = castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return Mathf.RoundToInt(damage);
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

    private void PlayCustomEarthquakeVFX(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        var directedAOEPattern = _pattern as DirectedAOEPattern;

        directedAOEPattern.SetDirection(CalculateDirection(casterTile, targetTile));
        directedAOEPattern.SetCasterTile(casterTile);

        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);
        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile != null)
            {
                if (_abilityAOEVFXSequence != null)
                {
                    VFXData data = new VFXData
                    {
                        Caster = GetCharacterCaster(),
                        OriginPosition = casterTile.transform.position,
                        TargetTile = tile,
                        TargetPosition = tile.transform.position,
                        Direction = (tile.transform.position - casterTile.transform.position).normalized,
                    };
                    data.Caster.StartCoroutine(_abilityAOEVFXSequence.RunSequence(data)
                    );
                }
            }
        }
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
