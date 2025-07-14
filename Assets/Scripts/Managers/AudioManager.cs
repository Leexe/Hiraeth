using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class AudioManager : PersistantSingleton<AudioManager> {
	private float _masterVolume = 1f;
	private float _musicVolume = 1f;
	private float _gameVolume = 1f;

	private Bus _masterBus;
	private Bus _musicBus;
	private Bus _gameBus;

	private List<EventInstance> _eventInstances;
	private List<StudioEventEmitter> _eventEmitters;

	// Music Instances
	private EventInstance _tutorialMusicInstance;
	private EventInstance _level1MusicInstance;
	private EventInstance _level2MusicInstance;
	private EventInstance _level3MusicInstance;
	private EventInstance _currentMusicTrack;

	protected override void Awake() {
		base.Awake();
		_eventInstances = new List<EventInstance>();
		_eventEmitters = new List<StudioEventEmitter>();

		_masterBus = RuntimeManager.GetBus("bus:/");
		_musicBus = RuntimeManager.GetBus("bus:/Music");
		_gameBus = RuntimeManager.GetBus("bus:/SFX");

		// Fetch audio preferences
		_masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
		_musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
		_gameVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
	}

	private void Start() {
		// Assign music instances
		_tutorialMusicInstance = CreateInstance(FMODEvents.Instance.HiraethAmbient);
		_level1MusicInstance = CreateInstance(FMODEvents.Instance.HiraethTrack2);
		_level2MusicInstance = CreateInstance(FMODEvents.Instance.HiraethTrack3);
		_level3MusicInstance = CreateInstance(FMODEvents.Instance.HiraethTrack4);

		// Event Listeners
		LevelManger.Instance.OnNewSceneLoaded.AddListener(HandleMusicOnSceneChange);

		// Play the music of the level on start up
		HandleMusicOnSceneChange(LevelManger.Instance.CurrentLevel);
	}

	private void Update() {
		_masterBus.setVolume(_masterVolume);
		_musicBus.setVolume(_musicVolume);
		_gameBus.setVolume(_gameVolume);
	}

	private void OnDisable() {
		SaveAudioPref();
	}

	private void OnDestroy() {
		CleanUp();
	}

	private void HandleMusicOnSceneChange(LevelManger.Level level) {
		_currentMusicTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		// Set the track to a level track if possible
		if (level == LevelManger.Level.mainMenu) {
			return;
		}
		else if (level == LevelManger.Level.tutorial) {
			_currentMusicTrack = _tutorialMusicInstance;
		}
		else if (level == LevelManger.Level.level1) {
			_currentMusicTrack = _level1MusicInstance;
		}
		else if (level == LevelManger.Level.level2) {
			_currentMusicTrack = _level2MusicInstance;
		}
		else if (level == LevelManger.Level.level3) {
			_currentMusicTrack = _level3MusicInstance;
		}

		_currentMusicTrack.setTimelinePosition(0);
		_currentMusicTrack.start();
	}

	// Plays a sound effect at a position in the world
	public void PlayOneShot(EventReference sound, Vector3 worldPos) {
		RuntimeManager.PlayOneShot(sound, worldPos);
	}

	// Plays a sound effect at a position in the world
	public void PlayOneShotAttached(EventReference sound, GameObject gameObject) {
		RuntimeManager.PlayOneShotAttached(sound, gameObject);
	}

	public void PlayOneShotAtPos(EventReference sound, Vector3 pos) {
		RuntimeManager.PlayOneShot(sound, position: pos);
	}

	public StudioEventEmitter InitalizeEventEmitter(EventReference eventReference, GameObject gameObject) {
		StudioEventEmitter emitter = gameObject.GetComponent<StudioEventEmitter>();
		emitter.EventReference = eventReference;
		_eventEmitters.Add(emitter);
		return emitter;
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

	// Delete an event instance
	public void DeleteInstance(EventInstance eventInstance) {
		_eventInstances.Remove(eventInstance);
		eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		eventInstance.release();
	}

	// Returns a boolean indicating if an instance is playing
	public bool InstanceIsPlaying(EventInstance instance) {
		PLAYBACK_STATE state;
		instance.getPlaybackState(out state);
		return state != PLAYBACK_STATE.STOPPED;
	}

	// Cleans up sound events and event emitters
	private void CleanUp() {
		foreach (EventInstance eventInstance in _eventInstances) {
			eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			eventInstance.release();
		}
		foreach (StudioEventEmitter emitter in _eventEmitters) {
			emitter.Stop();
		}
	}

	// Save audio preferences to file
	public void SaveAudioPref() {
		PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
		PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
		PlayerPrefs.SetFloat("GameVolume", _gameVolume);
	}

	// Getters and Setters
	public float GetMasterVolume() => _masterVolume;
	public float GetMusicVolume() => _musicVolume;
	public float GetGameVolume() => _gameVolume;
	public void SetMasterVolume(float masterVolume) => _masterVolume = Mathf.Clamp(masterVolume, 0f, 1f);
	public void SetMusicVolume(float musicVolume) => _musicVolume = Mathf.Clamp(musicVolume, 0f, 1f);
	public void SetGameVolume(float gameVolume) => _gameVolume = Mathf.Clamp(gameVolume, 0f, 1f);
}
