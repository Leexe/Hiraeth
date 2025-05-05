using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager> {  
    private List<EventInstance> _eventInstances;

    protected override void Awake() {
        base.Awake();
        _eventInstances = new List<EventInstance>();
    }
    
    private void OnDestroy() {
        CleanUp();
    }

    // Plays a sound effect at a position in the world
    public void PlayOneShot(EventReference sound, Vector3 worldPos) {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    // Plays a sound effect at a position in the world
    public void PlayOneShotAttached(EventReference sound, GameObject gameObject) {
        RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }

    // Creates an instance of a sound event
    public EventInstance CreateInstance(EventReference eventReference) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }

    // Creates an instance of a sound event and attaches it to a game object
    public EventInstance CreateInstanceAndAttach(EventReference eventReference, GameObject gameObject) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }

    // Cleans up sound events 
    private void CleanUp() {
        foreach (EventInstance eventInstance in _eventInstances) {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
}
