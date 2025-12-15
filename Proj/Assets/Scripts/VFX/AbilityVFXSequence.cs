using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXSequence", menuName = "Scriptable Objects/VFX/Sequences/BaseSequence")]

public class AbilityVFXSequence : ScriptableObject
{
    [SerializeField] private VFXPlayer _castFX;
    [SerializeField] private ProjectileVFXPlayer _travelFX;   // projectile or null
    [SerializeField] private VFXPlayer _impactFX;
    [SerializeField] private float _biggerImpactScale = 2f;
    [SerializeField] private float _offsetDistance;
    [SerializeField] private float _airDistance;
    [SerializeField] private Vector3 _rotationOffset;

    [SerializeField] private float _impactAirDistance;
    [SerializeField] private Vector3 _impactRotationOffset;

    public virtual IEnumerator RunSequence(VFXData data)
    {
        yield return new WaitForSeconds(data.CastingAnimationDuration);

        if (_castFX != null)
        {
            Vector3 offset = -data.Direction.normalized * _offsetDistance;
            offset.y = GetAirDistance();

            Instantiate(_castFX).Play(data.OriginPosition + offset, data.Direction, _rotationOffset);
        }
        yield return new WaitForSeconds(data.CastingFXDuration);

        if (_travelFX != null)
        {
            var projectile = Instantiate(_travelFX);
            yield return projectile.PlayProjectile(data.OriginPosition,data.TargetPosition
            );
        }

        yield return new WaitForSeconds(data.TravelFXDuration);


        if (_impactFX != null)
        {
            data.TargetPosition.y += GetImpactAirDistance();
            var impact = Instantiate(_impactFX);
            Vector3 baseScale = impact.transform.localScale;
            impact.transform.localScale = data.AoEDelta == 0 ? baseScale : Vector3.one * _biggerImpactScale;
            impact.Play(data.TargetPosition, data.Direction, _impactRotationOffset);
        }
    }

    public VFXPlayer GetCastFX() => _castFX;
 
    public ProjectileVFXPlayer GetTravelFX() => _travelFX;
   
    public VFXPlayer GetImpactFX() => _impactFX;
   
    public float GetOffsetDistance() => _offsetDistance;

    public float GetBiggerImpactScale() => _biggerImpactScale;

    public float GetAirDistance() => _airDistance;
   
    public Vector3 GetRotationOffset() => _rotationOffset;

    public float GetImpactAirDistance() => _impactAirDistance;

    public Vector3 GetImpactRotationOffset() => _impactRotationOffset;

}
