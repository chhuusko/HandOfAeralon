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

        affectedCharacter.TakeDamage(CalculateDamage(castingCharacter, affectedCharacter));
        affectedCharacter.GetStatusEffectManager().AddStatusEffect(new Vulnerable(_posionStacksToApply));

        if(affectedCharacter.TryGetComponent<StatusEffectManager>(out var statusEffectManager)){
            //if (statusEffectManager.ContainsStatusEffect<Poison>(){
               // CardHandManager.GetInstance().AddCardFromDeck();
            //}
            //statusEffectManager.AddStatusEffect(new Poison(_poisonStacksToApply);
            
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

        //1.
        int damage = castingCharacter.GetBaseDamage();

        //2.
       damage = (int) (damage * _damageMultiplier);

        //3-5.
        // damage = castingCharacter.GetStatusEffectManager().ModifyOutgoingDamage(damage, this);
        return damage;

        // 6-7 Gets applied withing affected character StatusEffectManager: ModifyOutgoingDamage(damage, this);
    }

    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}
