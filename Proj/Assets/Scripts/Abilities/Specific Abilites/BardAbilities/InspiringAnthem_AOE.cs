using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspiringAnthem_Ability", menuName = "Scriptable Objects/Abilities/Bard/Inspiring Anthem")]
public class InspiringAnthem_AOE : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private int _hasteStacks = 2;
    [SerializeField] private int _manaGain = 1;
    [SerializeField] private int _alliesBuffedTilBonus = 3;

    // Description

    // All allies in the target area gain Haste for 2 turns.
    // Gain 1 Mana if at least three allies gain Haste.


    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        int alliesBuffed = 0;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            alliesBuffed++;
            ApplyEffectOnTile(casterTile, tile);
        }

        // Check to see if casting character is friendly before changing mana.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (alliesBuffed >= _alliesBuffedTilBonus)
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

        StatusEffect haste = null;
        if(affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager)){
           statusEffectManager.AddStatusEffect(haste = new Haste(_hasteStacks));
        }
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, haste);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Not implemented.
    }
}
