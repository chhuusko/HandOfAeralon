using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/FloatModifierData")]
public class FloatModifierData : StatusEffectData
{
    [Range(0, 100)]public float ModifierPercent;
}
