using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(fileName = "Skullsplitter_Ability", menuName = "Scriptable Objects/Abilities/Range Calculations/Non-Blocking")]
public class Skullsplitter_Ability : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float damageMultiplier = 1.6f;


    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if (tileToEffect == null) return;

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;
        Character castingCharacter = casterTile.GetOccupantCharacter();
        if (castingCharacter == null) return;

        affectedCharacter.TakeDamage(CalculateDamage(castingCharacter, affectedCharacter));
    }

    private int CalculateDamage(Character castingCharacter, Character affectedCharacter)
    {
        // 1. Your Base Damage(Kan ökas med traits och eller kort.)
        // 2. Ability damage.
        // 3. Your Traits
        // 4. Your Buffs/ Debuffs(Kan även appliceras av kort)
        // 5. Eventuella Ability Global Modifiers(Ex.Arena modifiers)
        // 6. EnemyTraits
        // 7. Enemy Buffs / Debuffs

        //1.
        int damage = castingCharacter.GetBaseDamage();

        //2.
        damage = affectedCharacter.GetCurrentHealth() < (0.5 * affectedCharacter.GetMaxHealth()) ? (int) (damage*damageMultiplier) : damage;

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
