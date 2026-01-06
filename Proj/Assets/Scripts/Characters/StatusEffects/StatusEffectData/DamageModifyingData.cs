using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/DamageModifyingData")]
public class DamageModifyingData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    [Range(0, 100)]public float DamageModifierPercent;
}
