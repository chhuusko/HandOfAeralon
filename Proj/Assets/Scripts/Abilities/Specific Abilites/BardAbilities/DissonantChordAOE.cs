using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DissonantChord_Ability", menuName = "Scriptable Objects/Abilities/Bard/Dissonant Chord")]

public class DissonantChordAOE : RoundAOEAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private int _hasteStacks = 2;
    [SerializeField] private int _manaGain = 1;
    [SerializeField] private int _enemiseDebuffedTilBonus = 2;

    // Description

    // Emit a discordant note that dispels all buffs from characters in the area.
    // Draw 1 card if at least two buffs are dispelled from enemies.

    int enemiesDebuffed;

    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Calculate all tiles around with in radius and apply effect to all of them.
        if (_pattern is RoundAOEPattern pattern)
        {
            pattern.SetRadius(_radius);
        }
        List<CombatGridTile> tilesToEffect = _pattern.CalculateTilesToEffect(targetTile);

        enemiesDebuffed = 0;

        foreach (CombatGridTile tile in tilesToEffect)
        {
            if (tile == null) continue;
            if (!IsValidTargetForAbility(casterTile, tile)) continue;

            ApplyEffectOnTile(casterTile, tile);
        }

        // Check to see if casting character is friendly before drawing card.
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null || (castingCharacter.GetFaction() != Faction.Friendly)) return;

        if (enemiesDebuffed >= _enemiseDebuffedTilBonus)
        {
            CardHandManager.GetInstance().AddCardFromDeck();
        }
    }

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        if (affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager))
        {
           // statusEffectManager.);

            if(castingCharacter.GetFaction() != affectedCharacter.GetFaction()){
                enemiesDebuffed++;
            }
        }
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, 0, 0, null);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Not implemented.
    }
}
