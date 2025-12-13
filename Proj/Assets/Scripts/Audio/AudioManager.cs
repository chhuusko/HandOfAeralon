using System;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    private List<EventInstance> events;
    
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
