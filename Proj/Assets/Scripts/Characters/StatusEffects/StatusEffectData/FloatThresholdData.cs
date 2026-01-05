using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/FloatThresholdData")]
public class FloatThresholdData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    [Range(0, 100)]public float ThresholdPercent;
    public int TurnAmount;
}
