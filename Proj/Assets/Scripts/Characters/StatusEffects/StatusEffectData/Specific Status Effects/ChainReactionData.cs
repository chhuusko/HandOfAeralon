using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData/ChainReactionData")]
public class ChainReactionData : StatusEffectData
{
    [SerializeField, Range(0f, 100f)] private float applicationChancePercent;
    [SerializeField, UnityEngine.Min(0)] private int range;
    [SerializeField, UnityEngine.Min(1)] private int duration;
    public float ApplicationChancePercent => applicationChancePercent;
    public int Range => range;
    public int Duration => duration;
}
