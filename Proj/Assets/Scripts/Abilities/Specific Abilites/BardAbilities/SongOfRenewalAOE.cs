using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "SongOfRenewal_Ability", menuName = "Scriptable Objects/Abilities/Bard/SongOfRenewal")]
public class SongOfRenewalAOE : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _maxHealthHealMain = 0.25f;
    [SerializeField] private float _maxHealthHealArea = 0.1f;
    [SerializeField] private float _maxHealthSelfDamage = 0.1f;

    // Description

    // Restore 25% of a target ally’s max Health, and 10% to all allies in the area.
    // Draw 1 card if the main target was below 50% Health.


    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        if(casterTile == null || targetTile == null) return;

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

            if (tile == targetTile)
            {
                ApplyEffectOnMainTile(casterTile, tile);
                continue;
            }
            ApplyEffectOnTile(casterTile, tile);
        }

        Character caster = casterTile.GetOccupantCharacter();
        if (caster == null) return;
        caster.ShowPreviewVFX();
        GetAbilityHandler().AddPreviewedCharacter(caster);

        float damageAmount = caster.Data.DerivedHealthPoints * _maxHealthSelfDamage;
        damageAmount = caster.GetStatusEffectManager().ModifyOutgoingDamage(damageAmount, this);
        damageAmount = caster.GetStatusEffectManager().ModifyIncomingDamage(damageAmount, this);
        int damage = Mathf.RoundToInt(damageAmount);
        bool died = caster.TakeDamage(Mathf.RoundToInt(damage));

        AbilityExecutionData executionData = AbilityExecutionData.Create(this, caster, caster, casterTile, damage, 0, null, died);
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
            if (tile == null) continue;

            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            Character character;
            if (tile == targetTile)
            {
                PreviewSongOfRenewalOnTile(casterTile, tile, true);
                character = tile.GetOccupantCharacter();
                if (character == null) continue;
                character.ShowPreviewVFX();
                GetAbilityHandler().AddPreviewedCharacter(character);
                continue;
            }
            PreviewSongOfRenewalOnTile(casterTile, tile, false);
            character = tile.GetOccupantCharacter();
            if (character == null) continue;
            character.ShowPreviewVFX();
            GetAbilityHandler().AddPreviewedCharacter(character);
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

        int totalHealth = healAmount + affectedCharacter.Data.CurrentHealthPoints;
        if (totalHealth >= affectedCharacter.Data.DerivedHealthPoints)
        {
            healAmount = affectedCharacter.Data.DerivedHealthPoints - affectedCharacter.Data.CurrentHealthPoints;
        }

        if (healAmount <= 0) return;
        affectedCharacter.Heal(healAmount);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, healAmount, null, false);
    }

    private void ApplyEffectOnMainTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int healAmount = CalculateHealAmount(castingCharacter, affectedCharacter, true);

        int totalHealth = healAmount + affectedCharacter.Data.CurrentHealthPoints;
        if(totalHealth >= affectedCharacter.Data.DerivedHealthPoints)
        {
            healAmount = affectedCharacter.Data.DerivedHealthPoints - affectedCharacter.Data.CurrentHealthPoints;
        }

        if (healAmount <= 0) return;
        affectedCharacter.Heal(healAmount);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, healAmount, null, false);
    }

    private void PreviewSongOfRenewalOnTile(CombatGridTile casterTile, CombatGridTile targetTile, bool bIsMainTarget)
    {
        if (targetTile == null) return;

        Character affectedCharacter = targetTile.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int healAmount = CalculateHealAmount(castingCharacter, affectedCharacter, bIsMainTarget);

        int totalHealth = healAmount + affectedCharacter.Data.CurrentHealthPoints;
        if (totalHealth >= affectedCharacter.Data.DerivedHealthPoints)
        {
            healAmount = affectedCharacter.Data.DerivedHealthPoints - affectedCharacter.Data.CurrentHealthPoints;
        }

        if (healAmount == 0) return;
        affectedCharacter.PreviewHealthChange(healAmount);
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
        float healAmount = (affectedCharacter.Data.DerivedHealthPoints * healMultiplier);

        if (bIsMainTarget)
        {
            // Draw an extra card from your deck if main target was below 50% health and casting Character is a not an enemy.
            if (castingCharacter.GetFaction() == Faction.Friendly && affectedCharacter.Data.DerivedHealthPoints <  affectedCharacter.Data.DerivedHealthPoints * 0.5f)
            {
                CardHandManager.GetInstance().AddCardFromDeck();
            }
        }

        healAmount =  castingCharacter.GetStatusEffectManager().ModifyOutgoingHeal(healAmount, this);
        healAmount =  affectedCharacter.GetStatusEffectManager().ModifyIncomingHeal(healAmount, this);
        
        return Mathf.RoundToInt(healAmount); 
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}
