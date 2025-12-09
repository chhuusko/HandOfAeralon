using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/IntThresholdData")]
public class IntThresholdData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public int Threshold;
    public int TurnAmount;
}
