using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXSequence", menuName = "Scriptable Objects/VFX/Sequences/BaseSequence")]

public class AbilityVFXSequence : ScriptableObject
{
    [Header("Cast")]
    [SerializeField] private VFXPlayer _castFX;
    [SerializeField] private ProjectileVFXPlayer _travelFX;   // projectile or null
    [SerializeField] private float _biggerImpactScale = 2f;
    [SerializeField] private float _offsetDistance;
    [SerializeField] private float _airDistance;
    [SerializeField] private Vector3 _rotationOffset;

    [Header("Cast Shake")]
    [SerializeField] protected bool _doCastCameraShake = false;
    [SerializeField] protected float _castShakeDuration = 1.5f;
    [SerializeField] protected float _castShakeMagnitude = 0.1f;
    [SerializeField] protected float _castShakeFrequency = 200f;
    [SerializeField] protected AnimationCurve _castShakeFade;

    [Header("Impact")]
    [SerializeField] private VFXPlayer _impactFX;
    [SerializeField] private float _impactAirDistance;
    [SerializeField] private Vector3 _impactRotationOffset;

    [Header("Impact Shake")]
    [SerializeField] protected bool _doImpactCameraShake = false;
    [SerializeField] protected float _impactShakeDuration = 1.5f;
    [SerializeField] protected float _impactShakeMagnitude = 0.1f;
    [SerializeField] protected float _impactShakeFrequency = 200f;
    [SerializeField] protected AnimationCurve _impactShakeFade;


    public virtual IEnumerator RunSequence(VFXData data)
    {
        yield return new WaitForSeconds(data.CastingAnimationDuration);

        PlayShake(_doCastCameraShake, _castShakeDuration, _castShakeMagnitude, _castShakeFrequency, _castShakeFade);
        if (_castFX != null)
        {
            Vector3 offset = -data.Direction.normalized * _offsetDistance;
            offset.y = GetAirDistance();

            Instantiate(_castFX).Play(data.Caster.transform.position + offset, data.Direction, _rotationOffset);
        }
        yield return new WaitForSeconds(data.CastingFXDuration);

        if (_travelFX != null)
        {
            var projectile = Instantiate(_travelFX);
            yield return projectile.PlayProjectile(data.OriginPosition, data.TargetPosition
            );
        }

        yield return new WaitForSeconds(data.TravelFXDuration);


        PlayShake(_doImpactCameraShake, _impactShakeDuration, _impactShakeMagnitude, _impactShakeFrequency, _impactShakeFade);
        if (_impactFX != null)
        {
            data.TargetPosition.y += GetImpactAirDistance();
            var impact = Instantiate(_impactFX);
            Vector3 baseScale = impact.transform.localScale;
            impact.transform.localScale = data.AoEDelta == 0 ? baseScale : Vector3.one * _biggerImpactScale;
            impact.Play(data.TargetPosition, data.Direction, _impactRotationOffset);
        }

        yield return new WaitForSeconds(data.ImpactFXDuration);
    }

    protected void PlayShake(bool doCamerashake, float shakeDuration, float shakeMagnitude, float shakeFrequency, AnimationCurve shakeFade)
    {
        if (doCamerashake && CameraShakeManager._instance != null)
        {
           CameraShakeManager._instance.PlayShake(shakeDuration,shakeMagnitude,shakeFrequency,shakeFade);
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
