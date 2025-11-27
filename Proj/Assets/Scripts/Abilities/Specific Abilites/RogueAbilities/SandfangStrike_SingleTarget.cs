using UnityEngine;


[CreateAssetMenu(fileName = "SandfangStrike_Ability", menuName = "Scriptable Objects/Abilities/Rogue/Sandfang Strike")]
public class SandfangStrike_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damageMultiplier = 1.3f;
    [SerializeField] private float _applyPoisonChance = 0.8f;
    [SerializeField] private int _posionStacksToApply = 3;


    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        int damage = CalculateDamage(castingCharacter, affectedCharacter);
        affectedCharacter.TakeDamage(damage);
        AbilityExecutionData executionData = AbilityExecutionData.Create(this, castingCharacter, affectedCharacter, tileToEffect, damage, 0);

        if(affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager)){
            if (castingCharacter.GetFaction() == Faction.Friendly && statusEffectManager.ContainsStatusEffect<Poison>()){
                CardHandManager.GetInstance().AddCardFromDeck();
            }
            statusEffectManager.AddStatusEffect(new Poison(_posionStacksToApply));
        }
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // 1. Your Base Damage(Can also be applied by traits or cards.)
        // 2. Ability damage.
        // 3. Your Traits
        // 4. Your Buffs/ Debuffs(Can also be applied by cards.)
        // 5. Eventuella Ability Global Modifiers(Ex.Arena modifiers)
        // 6. EnemyTraits
        // 7. Enemy Buffs / Debuffs

     
        int damage = castingCharacter.GetBaseDamage();
        damage = (int)(damage * _damageMultiplier);


        damage = (int)castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        damage = (int)affectedCharacter.GetStatusEffectManager().ModifyIncomingDamage(damage, this);
        return damage;
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}
