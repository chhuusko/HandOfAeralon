using UnityEngine;
using System.Collections;

public class ProjectileVFXPlayer : VFXPlayer
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private VFXPlayer impactVFX;

    private Vector3 target;

    public void PlayProjectile(Vector3 start, Vector3 end)
    {
        transform.position = start;
        target = end;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }

        if (impactVFX != null)
        {
            var impact = Instantiate(impactVFX);
            impact.Play(target);
        }

        Destroy(gameObject);
    }
}
