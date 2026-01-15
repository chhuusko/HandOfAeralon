using System;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    private List<EventInstance> events;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        events = new List<EventInstance>();

    }

    private void Start()
    {
        if(RuntimeManager.GetBus("bus:/").isValid())
            masterBus = RuntimeManager.GetBus("bus:/");

        if(RuntimeManager.GetBus("bus:/Music").isValid())
            musicBus = RuntimeManager.GetBus("bus:/Music");
        
        if (RuntimeManager.GetBus("bus:/Music").isValid())
            sfxBus = RuntimeManager.GetBus("bus:/SoundEffect");
    }

    public void SetMasterVolume(float volume)
    {
        if (masterBus.isValid())
            masterBus.setVolume(volume);
    }
    public void SetMusicVolume(float volume)
    {
        if (musicBus.isValid())
            musicBus.setVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        if(sfxBus.isValid())
            sfxBus.setVolume(volume);
    }
    public void PlayOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }

    public void PlayParameterizedOneShot(EventReference sound, Vector3 position, string parameterName, float parameterValue)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.setParameterByName(parameterName, parameterValue);

        instance.start();
        instance.release();
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        events.Add(eventInstance);
        
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in events)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
