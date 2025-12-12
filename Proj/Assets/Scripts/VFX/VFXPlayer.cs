using PilotoStudio;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private ParticleHandler particleHandler;
    [SerializeField] private float fallbackLifetime = 5f;

    public void Play(Vector3 position, Vector3? direction = null, Vector3? rotationOffset = null)
    {
        transform.position = position;

        if (direction.HasValue && direction.Value != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction.Value);

        if(rotationOffset.HasValue && rotationOffset.Value != Vector3.zero)
        transform.rotation *= Quaternion.Euler((Vector3)rotationOffset);


        if (particleHandler != null)
        {
            particleHandler.Cast();
            Destroy(gameObject, EstimateLifetime());
        }
        else
        {
            foreach (var ps in GetComponentsInChildren<ParticleSystem>())
                ps.Play();

            Destroy(gameObject, fallbackLifetime);
        }
    }

    private float EstimateLifetime()
    {
        return fallbackLifetime;
    }
}
