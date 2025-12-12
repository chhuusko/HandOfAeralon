using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/StealthData")]
public class StealthData : StatusEffectData
{
    [Header("Effect-Specific Data")] 
    public float DamageModifier;
    public int MovementPointModifier;
}
