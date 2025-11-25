using UnityEngine;

public class DamagePopupText : MonoBehaviour
{
    [SerializeField] private float _destroyTime;
    [SerializeField] private Vector3 _randomStartPositionRange; 

    private void OnEnable()
    {
    }

    void Start()
    {
        Destroy(gameObject, _destroyTime);
        float randomStartPositionX = Random.Range(-_randomStartPositionRange.x, _randomStartPositionRange.x);
        float randomStartPositionY = Random.Range(-_randomStartPositionRange.y, _randomStartPositionRange.y);
        float randomStartPositionZ = Random.Range(-_randomStartPositionRange.z, _randomStartPositionRange.z);

        Vector3 randomPos = new Vector3(randomStartPositionX, randomStartPositionY, randomStartPositionZ);
        transform.localPosition += randomPos;
    }

    void Update()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = transform.position.y; // lock vertical tilt

        transform.LookAt(camPos);
        transform.Rotate(0, 180, 0);
    }

    private void BindToHealthChanged(Character character)
    {
        character.OnHealthChanged += HealthChanged;
    }

    private void UnBindToHealthChanged(Character character)
    {
        character.OnHealthChanged -= HealthChanged;
    }

    private void HealthChanged(int newHealth, int amount)
    {

    }
}

