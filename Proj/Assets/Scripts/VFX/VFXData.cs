using UnityEngine;

public struct VFXData
{
    public Character Caster;
    public CombatGridTile TargetTile;
    public Vector3 TargetPosition;
    public Vector3 OriginPosition;
    public Vector3 Direction;
    public float CastingFXDuration, TravelFXDuration, ImpactFXDuration;
    public int AoEDelta;
}
