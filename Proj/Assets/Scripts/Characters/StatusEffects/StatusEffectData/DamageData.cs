using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/DamageData")]
public class DamageData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public int Damage;
}
