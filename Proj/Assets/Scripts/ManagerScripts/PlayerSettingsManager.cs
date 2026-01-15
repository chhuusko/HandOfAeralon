using UnityEngine;

public class PlayerSettingsManager : MonoBehaviour
{
    static private PlayerSettingsManager _instance;

    [Header("UI")]
    [SerializeField] private float _hoverLockSpeed;


    [Header("Audio")]
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;

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

    public void SetMasterVolume(float volume) 
    { 
        _masterVolume = volume; 
    }
    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
    }
    public float GetSFXVolume()
    {
        return _sfxVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
    }


}
