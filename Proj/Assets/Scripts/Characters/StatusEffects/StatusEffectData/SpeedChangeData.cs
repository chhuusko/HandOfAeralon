using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/SpeedChangeData")]
public class SpeedChangeData : StatusEffectData
{
    [Header("Effect-Specific Data")]
    public int MovementPoints;
    public int Initiative;
}
