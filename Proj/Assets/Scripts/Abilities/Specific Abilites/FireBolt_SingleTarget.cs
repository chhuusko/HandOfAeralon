using UnityEngine;

[CreateAssetMenu(fileName = "Firebolt_Ability", menuName = "Scriptable Objects/Abilities/Single Target/Firebolt")]
public class Firebolt_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private int _damage;

    protected override void ApplyEffectOnTile(CombatGridTile casterTile, CombatGridTile tileToEffect)
    {
        if(tileToEffect == null) return;   

        Character affectedCharacter = tileToEffect.GetOccupantCharacter();
        if (affectedCharacter == null) return;

        affectedCharacter.TakeDamage(_damage);
    }
    protected override void InitiateParticles(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        // Spawn and direct VFX to target location.
    }
}
