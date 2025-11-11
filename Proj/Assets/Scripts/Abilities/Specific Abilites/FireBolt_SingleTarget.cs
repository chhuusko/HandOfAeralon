using UnityEngine;

public class FireBolt_SingleTarget : SingleTargetAbility
{
    [Header("- Ability Specific values -")]
    [SerializeField] private float _damage;

    protected override void ApplyEffectOnTile(CombatGridTile tileToEffect)
    {
       

    }
}
