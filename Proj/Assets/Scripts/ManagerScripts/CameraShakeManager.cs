using System.Collections;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager _instance {  get; private set; }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private IEnumerator Shake(float duration, float magnitude, float frequency, AnimationCurve fadeCurve)
    {
 
        if (fadeCurve == null)
        {
            fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        }

        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        float timeSinceLastShake = 0f;
        float interval = 1f / frequency;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            timeSinceLastShake += Time.deltaTime;

            if (timeSinceLastShake >= interval)
            {
                float t = elapsed / duration;
                float strength = magnitude * fadeCurve.Evaluate(t);

                float x = Random.Range(-1f, 1f) * strength;
                float y = Random.Range(-1f, 1f) * strength;

                transform.localPosition = originalPos + new Vector3(x, y, 0f);
                timeSinceLastShake = 0f;
            }

            yield return null;
        }

        transform.localPosition = originalPos;
    }
    public void PlayShake(float duration, float magnitude, float frequency, AnimationCurve fade)
    {
        StartCoroutine(Shake(duration, magnitude, frequency, fade));
    }
}
