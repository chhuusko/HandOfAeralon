using UnityEngine;


[CreateAssetMenu(fileName = "Emberwake_Ability", menuName = "Scriptable Objects/Abilities/Sorceress/Emberwake")]
public class Emberwake_Singletarget : SingleTargetAbility
{
    [SerializeField] private int _emberwakeDuration = 3;

    // Description

    // The Sorceress ignites her inner flame for 3 turns, causing all her abilities to gain a +25% chance to apply Burn.
    // During this effect, every time she applies Burn, reduce the cost of a random card in your hand by 1 (once per turn).

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        StatusEffectManager statusEffectManager = castingCharacter.GetComponent<StatusEffectManager>();
        if (statusEffectManager == null) return;

        StatusEffect emberwake;
        statusEffectManager.AddStatusEffect(emberwake = new Emberwake(_emberwakeDuration));

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, emberwake, false);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        //
    }
}
