using UnityEngine;
using FMODUnity;

public class FMODEvents : Singleton<FMODEvents> {
    [field: Header("Player Sounds")]
    [field: SerializeField] public EventReference AirJump_sfx {get; private set;}
    [field: SerializeField] public EventReference AirDash_sfx {get; private set;}
    [field: SerializeField] public EventReference GroundedDash_sfx {get; private set;}
    [field: SerializeField] public EventReference Jump_sfx {get; private set;}
    [field: SerializeField] public EventReference Land_sfx {get; private set;}
    [field: SerializeField] public EventReference Slide_sfx {get; private set;}
    
    [field: Header("Music")]
    [field: SerializeField] public EventReference MusicTrack {get; private set;}
}
