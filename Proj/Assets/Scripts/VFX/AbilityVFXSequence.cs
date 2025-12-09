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

    public virtual IEnumerator RunSequence(VFXData data)
    {
        if (_castFX != null)
        {
            Vector3 offset = -data.Direction.normalized * _offsetDistance;
            offset.y = GetAirDistance();

            Instantiate(_castFX).Play(data.OriginPosition + offset, data.Direction);
        }
        yield return new WaitForSeconds(data.CastingFXDuration);

        if (_travelFX != null)
        {
            var projectile = Instantiate(_travelFX);
            yield return projectile.PlayProjectile(data.OriginPosition,data.TargetPosition
            );
        }

        if (_impactFX != null)
        {
            var impact = Instantiate(_impactFX);
            Vector3 baseScale = impact.transform.localScale;
            impact.transform.localScale = data.AoEDelta == 0 ? baseScale : Vector3.one * _biggerImpactScale;
            impact.Play(data.TargetPosition, data.Direction);
        }
    }

    public VFXPlayer GetCastFX()
    {
        return _castFX;
    }
    public ProjectileVFXPlayer GetTravelFX()
    {
        return _travelFX;
    }
    public VFXPlayer GetImpactFX()
    {
        return _impactFX;
    }
    public float GetOffsetDistance()
    {
        return _offsetDistance;
    }
    public float GetAirDistance()
    {
        return _airDistance;
    }
}
