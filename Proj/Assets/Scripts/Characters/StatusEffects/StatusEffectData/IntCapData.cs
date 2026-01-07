using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/IntCapData")]
public class IntCapData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public int Cap;
    public int Damage;
}
