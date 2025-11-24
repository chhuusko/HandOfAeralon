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
    [SerializeField] private AbilityTargetType _targetType;

    [Header("- Visuals & Audio - ")]
    [SerializeField] private ParticleSystem castingEffect, hitEffect;
    [SerializeField] private AudioClip castingSound, hitSound;
    [SerializeField] private float castingTime, fromCastToHitTime;

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
        Debuff
    }

    public enum AbilityTargetType
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
    public AbilityTargetType GetAbilityTargetType() => _targetType;

    public RangeCalculation GetRangeCalculation => _rangeCalculation;

    public List<CombatGridTile> GetAvailableTargets(CombatGridTile casterTile)
    {
        return _rangeCalculation.CalculateTilesInRange(casterTile, _range);
    }

    public IEnumerator PlayAbilityEffect(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Play casting sound.
        yield return new WaitForSeconds(castingTime);
        InitiateParticles(casterTile, targetTile);
        // Play hit sound.
        yield return new WaitForSeconds(fromCastToHitTime);
    }
    protected abstract void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile);


}
