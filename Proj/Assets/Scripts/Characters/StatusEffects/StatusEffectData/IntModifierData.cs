using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/IntModifierData")]
public class IntModifierData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public int Modifier;
}
