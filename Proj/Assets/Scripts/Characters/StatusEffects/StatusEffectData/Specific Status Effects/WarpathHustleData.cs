using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/WarpathHustleData")]
public class WarpathHustleData : StatusEffectData
{
    [SerializeField, Min(0)] private int _movementPointModifier;
    [SerializeField, Min(0)] private int _movementPointCapacity;
    [SerializeField, Range(0f, 100f)] private float _damageModifierPercent;
    [SerializeField, Range(0f, 100f)] private float _damageModifierCapacity;
    public int MovementPointModifier => _movementPointModifier;
    public int MovementPointCapacity => _movementPointCapacity;
    public float DamageModifierPercent => _damageModifierPercent;
    public float DamageModifierCapacity => _damageModifierCapacity;
}
