using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "VeilOfDust_Ability", menuName = "Scriptable Objects/Abilities/Rogue/Veil Of Dust")]
public class VeilOfDust_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private int _buffsRemovedTilBonus = 1;
    [SerializeField] private int _stealthDuration = 3;
    [SerializeField] private int _hasteDuration = 3;
    [SerializeField] private int _manaGain = 1;

    // Description

    // Cleanse all debuffs from self and gain Stealth for 3 turns.
    // You can move after using this ability.
    // If a debuff is cleansed, gain 1 Mana.

    public override IEnumerator StartAbilityEffects(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Same as base class "Ability", but the movement points are not reset.

        Character caster = casterTile.GetOccupantCharacter();
        if (caster == null) Debug.LogError("CasterTile has no character!");

        caster.Animator.SetBool("AbilityOngoing", true);

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
            };
            yield return caster.StartCoroutine(GetAbilityVFXSequence().RunSequence(data)
            );
        }
        // Play hit sound.

        RunAbility(casterTile, targetTile);

        yield return new WaitForEndOfFrame();
        Selector._instance.SelectCharacterFromUI(caster);

        Selector._instance.InvokeCharacterActionStopped();
        CombatEventManager.InvokeAfterAbilityCast(caster, this);
        yield return new WaitForSeconds(3);
        caster.Animator.SetBool("AbilityOngoing", false);
    }

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        StatusEffectSystem statusEffectSystem = affectedCharacter.GetStatusEffectManager();
        if (statusEffectSystem == null) return;

        int effectsRemoved = statusEffectSystem.ClearStatusEffects(StatusEffectType.Debuff);

        StatusEffect stealth;
        statusEffectSystem.AddStatusEffect(stealth = new Stealth(_stealthDuration));

        statusEffectSystem.AddStatusEffect(new Haste(_hasteDuration));

        if (effectsRemoved >= _buffsRemovedTilBonus && castingCharacter.GetFaction() == Faction.Friendly)
        {
            CardHandManager.GetInstance().ChangeMana(_manaGain);
        }

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, stealth, false);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }

}
