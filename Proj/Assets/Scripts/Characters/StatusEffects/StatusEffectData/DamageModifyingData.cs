using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/DamageModifyingData")]
public class DamageModifyingData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public float DamageModifier;
}
