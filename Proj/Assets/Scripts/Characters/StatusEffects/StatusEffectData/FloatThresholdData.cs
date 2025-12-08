using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/FloatThresholdData")]
public class FloatThresholdData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public float Threshold;
    public int TurnAmount;
}
