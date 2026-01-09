using UnityEngine;

public class PlayerSettingsManager : MonoBehaviour
{
    static private PlayerSettingsManager _instance;

    [Header("UI")]
    [SerializeField] private float _hoverLockSpeed;


    [Header("Audio")]
    [SerializeField] private float _masterVolume;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
        
    }


    public static PlayerSettingsManager GetInstance() { return _instance; }


    //UI
    public float GetHoverLockSpeed() 
    { 
        return _hoverLockSpeed; 
    }
    public void SetHoverLockSpeed(float hoverLockSpeed) 
    { 
        _hoverLockSpeed = hoverLockSpeed; 
    }

    // AUDIO
    public float GetMasterVolume() 
    { 
        return _masterVolume; 
    }
    public void SetMasterVolume(float masterVolume) 
    { 
        _masterVolume = masterVolume; 
    }
}
