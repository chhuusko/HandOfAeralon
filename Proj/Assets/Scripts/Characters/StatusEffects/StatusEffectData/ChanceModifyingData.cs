using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/ChanceModifyingData")]
public class ChanceModifyingData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    [Range(0, 100)]public float ChancePercent;
    public int Modifier;
}
