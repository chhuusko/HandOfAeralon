using UnityEngine;

public class ParticleSpawnerManager : MonoBehaviour
{
    private static ParticleSpawnerManager _instance;

    [SerializeField] private ParticleSystem _poisonExplosion;
    [SerializeField] private ParticleSystem[] _lavaExplosion;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    void Start()
    {
        
    }

    public void SpawnPoisonExplosion(Vector3 positionToSpawnAt)
    {
        Instantiate(_poisonExplosion, positionToSpawnAt, Quaternion.identity);
    }

    public void SpawnLavaExplosion(Vector3 positionToSpawnAt)
    {
        Instantiate(_lavaExplosion[0], positionToSpawnAt, Quaternion.identity);
        Instantiate(_lavaExplosion[1], positionToSpawnAt, Quaternion.identity);
    }


    public static ParticleSpawnerManager GetInstance() { return _instance; }
}
