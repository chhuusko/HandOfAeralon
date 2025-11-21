using UnityEngine;

public enum StatusEffectType {Buff, Debuff, ArenaEffect, Trait, CrowdControl}

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffects/StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    public string Name;
    public int Duration;
    public Sprite Icon;
    public StatusEffectType Type;
    public bool IsPermanent;
}
