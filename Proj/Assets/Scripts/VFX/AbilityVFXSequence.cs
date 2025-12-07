using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class AbilityVFXSequence : ScriptableObject
{
    [SerializeField] private VFXPlayer castFX;
    [SerializeField] private ProjectileVFXPlayer travelFX;   // projectile or null
    [SerializeField] private VFXPlayer impactFX;

    public virtual IEnumerator RunSequence(VFXData data)
    {
        if (castFX != null)
        {
            Instantiate(castFX).Play(data.OriginPosition, data.Direction);
            yield return new WaitForSeconds(data.CastingFXDuration);
        }

        if (travelFX != null)
        {
            var projectile = Instantiate(travelFX);
            yield return projectile.PlayProjectile(data.OriginPosition,data.TargetPosition
            );
        }

        if (impactFX != null)
        {
            Instantiate(impactFX).Play(data.TargetPosition, data.Direction);
        }
    }
}
