using UnityEngine;
using System.Collections;

public class ProjectileVFXPlayer : VFXPlayer
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private VFXPlayer impactVFX;

    private Vector3 target;

    public Coroutine PlayProjectile(Vector3 start, Vector3 end)
    {
        transform.position = start;
        target = end;
        return StartCoroutine(Move());
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

        Destroy(gameObject);
    }
}
