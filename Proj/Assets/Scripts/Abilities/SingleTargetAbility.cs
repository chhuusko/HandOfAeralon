using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetAbility", menuName = "Scriptable Objects/Abilities/Single Target")]
public abstract class SingleTargetAbility : Ability
{
    public override void RunAbility(CombatGridTile casterTile, CombatGridTile targetTile)
    {
        ApplyEffectOnTile(targetTile);
    }

}
