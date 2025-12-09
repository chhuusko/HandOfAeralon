using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "DelayedVFXSequence", menuName = "Scriptable Objects/VFX/Sequences/DelayedSequence")]

public class AbilityVFXSequenceDelayed : AbilityVFXSequence
{
    public override IEnumerator RunSequence(VFXData data)
    {
        yield return new WaitForSeconds(data.CastingFXDuration);
        if (GetCastFX() != null)
        {
            Vector3 offset = -data.Direction.normalized * GetOffsetDistance();
            offset.y = GetAirDistance();
            Instantiate(GetCastFX()).Play(data.OriginPosition + offset, data.Direction);
        }
        yield return new WaitForSeconds(data.TravelFXDuration);

        if (GetTravelFX() != null)
        {
            var projectile = Instantiate(GetTravelFX());
            yield return projectile.PlayProjectile(data.OriginPosition, data.TargetPosition
            );
        }

        if (GetImpactFX() != null)
        {
            var impact = Instantiate(GetImpactFX());
         
            impact.Play(data.TargetPosition, data.Direction);
        }
    }
}
