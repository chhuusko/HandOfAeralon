using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "DelayedVFXSequence", menuName = "Scriptable Objects/VFX/Sequences/DelayedSequence")]

public class AbilityVFXSequenceDelayed : AbilityVFXSequence
{
    public override IEnumerator RunSequence(VFXData data)
    {
        yield return new WaitForSeconds(data.CastingAnimationDuration);

        PlayShake(_doCastCameraShake, _castShakeDuration, _castShakeMagnitude, _castShakeFrequency, _castShakeFade);
        if (GetCastFX() != null)
        {
            Vector3 offset = -data.Direction.normalized * GetOffsetDistance();
            offset.y = GetAirDistance();
            Instantiate(GetCastFX()).Play(data.OriginPosition + offset, data.Direction);
        }
        yield return new WaitForSeconds(data.CastingFXDuration);

        if (GetTravelFX() != null)
        {
            var projectile = Instantiate(GetTravelFX());
            yield return projectile.PlayProjectile(data.OriginPosition, data.TargetPosition
            );
        }
        yield return new WaitForSeconds(data.TravelFXDuration);

        PlayShake(_doImpactCameraShake, _impactShakeDuration, _impactShakeMagnitude, _impactShakeFrequency, _impactShakeFade);
        if (GetImpactFX() != null)
        {

            var impact = Instantiate(GetImpactFX());

            impact.Play(data.TargetPosition, data.Direction);
        }

        yield return new WaitForSeconds(data.ImpactFXDuration);
    }
}
