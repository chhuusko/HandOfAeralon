using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/UnyieldingTremorData")]
public class UnyieldingTremorData : StatusEffectData
{
    [Range(0, 100)]public float CapPercent;
    [Range(0, 100)]public float ModifierPercent;
}
