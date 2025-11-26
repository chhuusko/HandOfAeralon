using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("- General -")]
    [SerializeField] private string _abilityName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _range;
    [SerializeField] private int _cooldown;

    [Header("- Tags -")]
    [SerializeField] private AbilityTag _abilityTag;

    [Header("- Types -")]
    [SerializeField] private Type _type;

    [Header("- Targeting -")]
    [SerializeField] private RangeCalculation _rangeCalculation;
    [SerializeField] private ValidTargetOccupant _targetType;

    [Header("- Visuals & Audio - ")]
    [SerializeField] private ParticleSystem _castingEffect, _hitEffect;
    [SerializeField] private AudioClip _castingSound, _hitSound;
    [SerializeField] private float _castingTime, _fromCastToHitTime;
    [SerializeField] private float _castingRotationTime = 0.3f;


    [System.Flags]
    public enum AbilityTag
    {
        None = 0,
        Melee = 1 << 0,
        Ranged = 1 << 1,
        SingleTarget = 1 << 2,
        AOE = 1 << 3
    }
    public enum Type
    {
        Physical,
        Elemental,
        Heal,
        Buff,
        Debuff,
        Movement
    }

    public enum ValidTargetOccupant
    {
        Any,
        CharacterOccupiedTile,
        Friendly,
        Enemy
    }

    public abstract void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile);
    public abstract List<CombatGridTile> GetTilesToEffect(CombatGridTile tile);
    protected abstract void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile targetTile);

    public string GetAbilityName() => _abilityName;
    public Sprite GetIcon() => _icon;
    public float GetRange() => _range;
    public int GetCooldown() => _cooldown;

    public void SetCooldown(int cooldown)
    {
        _cooldown = cooldown;
    }
    public ValidTargetOccupant GetAbilityTargetType() => _targetType;

    public RangeCalculation GetRangeCalculation => _rangeCalculation;

    public List<CombatGridTile> GetAvailableTargets(CombatGridTile casterTile)
    {
        return _rangeCalculation.CalculateTilesInRange(casterTile, _range);
    }

    public IEnumerator StartAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        Character caster = casterTile.GetOccupantCharacter();
        if (caster == null) Debug.LogError("CasterTile has no character!");
        ResetMovementPoints(caster);
        caster.RotateTowards(targetTile.transform, _castingRotationTime);

        if (caster.TryGetComponent<Animator>(out var animator)){
            animator.SetTrigger(_abilityName);
        }
        // Play Animation.
        // Play casting sound.
        yield return new WaitForSeconds(_castingTime);
        InitiateParticles(casterTile, targetTile);
        // Play hit sound.
        yield return new WaitForSeconds(_fromCastToHitTime);
        RunAbility(casterTile, targetTile);
    }
    protected void ResetMovementPoints(Character character)
    {
        character.SetCurrentMovementPoints(0);
    }
    protected abstract void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile);}
