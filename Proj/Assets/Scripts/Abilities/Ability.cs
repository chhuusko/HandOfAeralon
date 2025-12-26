using FMODUnity;
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
    [SerializeField] private string _description;

    [Header("- Tags -")]
    [SerializeField] private AbilityTag _abilityTag;

    [Header("- Types -")]
    [SerializeField] private Type _type;

    [Header("- Targeting -")]
    [SerializeField] private RangeCalculation _rangeCalculation;
    [SerializeField] private ValidTargetOccupant _targetType;

    [Header("- Visuals & Audio - ")]
    [SerializeField] private AbilityVFXSequence _abilityVFXSequence;
    [SerializeField] private AudioClip _castingSound, _hitSound;
    [SerializeField] private float _castingAnimationTime, _castingFXTime, _fromCastToHitTime, _impactVFXTime;
    [SerializeField] private float _castingRotationTime = 0.3f;
    [field: SerializeField] public EventReference AudioEvent { get; private set; }

    private AbilityHandler _abilityHandler;
    private Character _characterCaster;


    public AbilityHandler GetAbilityHandler() => _abilityHandler;
    public Character GetCharacterCaster() => _characterCaster;

    public AbilityVFXSequence GetAbilityVFXSequence() => _abilityVFXSequence;


    public void SetAbilityHandler(AbilityHandler abilityHandler)
    {
        _abilityHandler = abilityHandler;
    }
    public void SetCharacterCaster(Character caster)
    {
        _characterCaster = caster;
    }


    [System.Flags]
    public enum AbilityTag
    {
        None = 0,
        Melee = 1 << 0,
        Ranged = 1 << 1,
        SingleTarget = 1 << 2,
        AOE = 1 << 3
    }
    [System.Flags]
    public enum Type
    {
        None = 0,
        Physical = 1 << 0,
        Elemental = 1 << 1,
        Heal = 1 << 2,
        Buff = 1 << 3,
        Debuff = 1 << 4,
        Movement = 1 << 5
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
    public string GetDescription() => _description;
    public float GetCastingRotationTime() => _castingRotationTime;
    public float GetCastingAnimationTime() => _castingAnimationTime;
    public float GetCastingTime() => _castingFXTime;
    public float GetFromCastToHitTime() => _fromCastToHitTime;
    public float GetImpactTime() => _impactVFXTime;
    public void SetCooldown(int cooldown)
    {
        _cooldown = cooldown;
    }
    public ValidTargetOccupant GetAbilityTargetType() => _targetType;

    public RangeCalculation GetRangeCalculation() => _rangeCalculation;

    public List<CombatGridTile> GetAvailableTargets(CombatGridTile casterTile)
    {
        return _rangeCalculation.CalculateTilesInRange(casterTile, _range);
    }

    public Type GetAbilityType() => _type;

    public virtual IEnumerator StartAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        Character caster = casterTile.GetOccupantCharacter();
        if (caster == null) Debug.LogError("CasterTile has no character!");

        // Should not be able to move after performing ability.
        caster.CanMove = false;

        // Rotate towards target if the target is not the caster's tile.
        if (casterTile != targetTile)
        {
            caster.RotateTowards(targetTile.transform, _castingRotationTime);
        }

        if (caster.TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger(_abilityName);
        }
        AudioManager.Instance.PlayOneShot(AudioEvent, caster.transform.position);

        if (_abilityVFXSequence != null)
        {
            VFXData data = new VFXData
            {
                Caster = caster,
                OriginPosition = casterTile.transform.position,
                TargetTile = targetTile,
                TargetPosition = targetTile.transform.position,
                Direction = (targetTile.transform.position - casterTile.transform.position).normalized,
                CastingAnimationDuration = _castingAnimationTime,
                CastingFXDuration = _castingFXTime,
                TravelFXDuration = _fromCastToHitTime,
                ImpactFXDuration = _impactVFXTime
            };
            yield return caster.StartCoroutine(_abilityVFXSequence.RunSequence(data)
            );
        }

        RunAbility(casterTile, targetTile);

        Selector._instance.InvokeCharacterActionStopped();
    }
    protected void ResetMovementPoints(Character character)
    {
        character.SetCurrentMovementPoints(0);
    }
    protected abstract void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile);

    public virtual void PreviewAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Ability preview is overridden in subclasses if ability actually changes HP.
        return;
    }
    protected virtual void PreviewEffectOnTile(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Preview effect on tile is overridden in subclasses if ability actually changes HP.
        return;
    }

    public virtual int GetDamage()
    {
        return 0;
    }

    public virtual int GetSecondDamage()
    {
        return 0;
    }
}

