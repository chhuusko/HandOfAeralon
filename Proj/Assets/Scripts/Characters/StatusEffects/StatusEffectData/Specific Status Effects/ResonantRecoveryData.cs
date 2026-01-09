using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/ResonantRecoveryData")]
public class ResonantRecoveryData : DamageModifyingData
{
    [Range(0,100)]public float HealModifierPercent;
}
