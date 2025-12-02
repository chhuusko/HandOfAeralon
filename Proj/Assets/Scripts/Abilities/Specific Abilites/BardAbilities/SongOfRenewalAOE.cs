using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongOfRenewal_Ability", menuName = "Scriptable Objects/Abilities/Bard/SongOfRenewal")]
public class SongOfRenewalAOE : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _maxHealthHealMain = 0.25f;
    [SerializeField] private float _maxHealthHealArea = 0.1f;

    // Description

    // Restore 25% of a target ally’s max Health, and 10% to all allies in the area.
    // Draw 1 card if the main target was below 50% Health.


    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;

            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            if (tile == targetTile)
            {
                ApplyEffectOnMainTile(casterTile, tile);
                continue;
            }
            ApplyEffectOnTile(casterTile, tile);
        }
    }

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int healAmount = CalculateHealAmount(castingCharacter, affectedCharacter, false);
        affectedCharacter.Heal(healAmount);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, healAmount, null);
    }

    private void ApplyEffectOnMainTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int healAmount = CalculateHealAmount(castingCharacter, affectedCharacter, true);
        affectedCharacter.Heal(healAmount);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, healAmount, null);
    }

    private int CalculateHealAmount(Character castingCharacter, Character affectedCharacter, bool bIsMainTarget)
    {
        // 1. Character health
        // 2. Ability Heal
        // 3. Your Traits
        // 4. Your Buffs/ Debuffs(Kan även appliceras av kort)
        // 5. Eventuella Ability Global Modifiers(Ex.Arena modifiers)
        // 6. EnemyTraits
        // 7. Enemy Buffs / Debuffs

        //1.
        //2.
        float healMultiplier = bIsMainTarget ? _maxHealthHealMain : _maxHealthHealArea;
        int healAmount = (int) (affectedCharacter.GetMaxHealth() * healMultiplier);

        if (bIsMainTarget)
        {
            // Draw an extra card from your deck if main target was below 50% health and casting Character is a not an enemy.
            if (castingCharacter.GetFaction() == Faction.Friendly && affectedCharacter.GetCurrentHealth() < (int) (affectedCharacter.GetMaxHealth() * 0.5f))
            {
                CardHandManager.GetInstance().AddCardFromDeck();
            }
        }

        //3-5.
        // healAmount = castingCharacter.GetStatusEffectManager().ModifyOutgoingHeal(healAmount, this);

        // 6-7 Gets applied withing affected character StatusEffectManager: ModifyOutgoingHeal(healAmount, this);
        
        return healAmount; 
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}
