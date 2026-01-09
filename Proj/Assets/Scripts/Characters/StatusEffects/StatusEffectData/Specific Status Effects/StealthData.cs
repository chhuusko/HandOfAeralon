using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/StealthData")]
public class StealthData : DamageModifyingData
{
    [Header("Effect-Specific Data")] 
    public int MovementPointModifier;
}
