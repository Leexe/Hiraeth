using UnityEngine;
using FMODUnity;

public class FMODEvents : Singleton<FMODEvents> {
    [field: Header("Player Sounds")]
    [field: SerializeField] public EventReference AirJump_sfx {get; private set;}
    [field: SerializeField] public EventReference AirDash_sfx {get; private set;}
    [field: SerializeField] public EventReference GroundedDash_sfx {get; private set;}
    [field: SerializeField] public EventReference DownwardsDash_sfx {get; private set;}
    [field: SerializeField] public EventReference Jump_sfx {get; private set;}
    [field: SerializeField] public EventReference Land_sfx {get; private set;}
    [field: SerializeField] public EventReference Slide_sfx {get; private set;}
    [field: SerializeField] public EventReference GunShoot_sfx {get; private set;}
    [field: SerializeField] public EventReference Reload_sfx {get; private set;}
    [field: SerializeField] public EventReference WallRun_sfx {get; private set;}
    [field: SerializeField] public EventReference WallJump_sfx {get; private set;}
    [field: SerializeField] public EventReference PlayerHurt_sfx {get; private set;}

	[field: Header("Enemy Sounds")]
	[field: SerializeField] public EventReference EnemyDeath_sfx {get; private set;}
	[field: SerializeField] public EventReference EnemyIdle_sfx {get; private set;}
	[field: SerializeField] public EventReference EnemyAttack_sfx {get; private set;}

    [field: Header("Game Sounds")]
    [field: SerializeField] public EventReference GameStartUp_sfx {get; private set;}
    [field: SerializeField] public EventReference Lose_sfx {get; private set;}
    [field: SerializeField] public EventReference LevelComplete_sfx {get; private set;}
    [field: SerializeField] public EventReference BouncePad_sfx {get; private set;}
	[field: SerializeField] public EventReference Parry_sfx {get; private set;}

	[field: Header("Pause Sounds")]
    [field: SerializeField] public EventReference MasterVolume_sfx {get; private set;}
    [field: SerializeField] public EventReference GameVolume_sfx {get; private set;}
    [field: SerializeField] public EventReference MusicVolume_sfx {get; private set;}
    [field: SerializeField] public EventReference Pause_sfx {get; private set;}
    [field: SerializeField] public EventReference Unpause_sfx {get; private set;}
    [field: SerializeField] public EventReference PauseClick_sfx {get; private set;}
    [field: SerializeField] public EventReference PauseHover_sfx {get; private set;}

	[field: Header("Music")]
    [field: SerializeField] public EventReference HiraethAmbient {get; private set;}
    [field: SerializeField] public EventReference HiraethTrack2 {get; private set;}
    [field: SerializeField] public EventReference HiraethTrack3 {get; private set;}
    [field: SerializeField] public EventReference HiraethTrack4 {get; private set;}
}
