using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBasedSequence", menuName = "Scriptable Objects/VFX/Sequences/CharacterBasedSequence")]
public class AbilityVFXSequenceCharacterPos : AbilityVFXSequence
{
    [SerializeField] private VFXPlayer _startFX;


    public override IEnumerator RunSequence(VFXData data)
    {
        if(_startFX != null)
        {
            Instantiate(_startFX).Play(data.OriginPosition, Vector3.zero, Vector3.zero);
        }

        yield return new WaitForSeconds(data.CastingAnimationDuration);

        if (GetCastFX() != null)
        {
            PlayShake(_doCastCameraShake, _castShakeDuration, _castShakeMagnitude, _castShakeFrequency, _castShakeFade);
            Vector3 offset = -data.Direction.normalized * GetOffsetDistance();
            offset.y = GetAirDistance();

            Instantiate(GetCastFX()).Play(data.Caster.transform.position + offset, data.Direction, GetRotationOffset());
        }
        yield return new WaitForSeconds(data.CastingFXDuration);

        if (GetTravelFX() != null)
        {
            var projectile = Instantiate(GetTravelFX());
            yield return projectile.PlayProjectile(data.OriginPosition, data.TargetPosition
            );
        }

        yield return new WaitForSeconds(data.TravelFXDuration);

        if (GetImpactFX() != null)
        {
            PlayShake(_doImpactCameraShake, _impactShakeDuration, _impactShakeMagnitude, _impactShakeFrequency, _impactShakeFade);
            data.TargetPosition.y += GetImpactAirDistance();
            var impact = Instantiate(GetImpactFX());
            Vector3 baseScale = impact.transform.localScale;
            impact.transform.localScale = data.AoEDelta == 0 ? baseScale : Vector3.one * GetBiggerImpactScale();
            impact.Play(data.TargetPosition, data.Direction, GetImpactRotationOffset());
        }

        yield return new WaitForSeconds(data.ImpactFXDuration);
    }
}
