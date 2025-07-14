using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuSounds : MonoBehaviour {
	[Header("References")]
	[SerializeField] private SettingsUI _settingMenu;

	[Header("Sound Sliders")]
	[SerializeField] private Slider _masterVolumeSlider;
	[SerializeField] private Slider _gameVolumeSlider;
	[SerializeField] private Slider _musicVolumeSlider;

	// Sound Instances
	private EventInstance _masterTestInstance;
	private EventInstance _gameTestInstance;
	private EventInstance _musicTestInstance;

	private void Start() {
		// Add Event Listeners
		_settingMenu.OnButtonClick.AddListener(PlayButtonClickSound);

		// Create Audio Instances
		_masterTestInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.MasterVolume_sfx);
		_gameTestInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.GameVolume_sfx);
		_musicTestInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.MusicVolume_sfx);
		StopAllSounds();

		// Get Sound Settings
		_masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
		_gameVolumeSlider.value = AudioManager.Instance.GetGameVolume();
		_musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
	}

	private void OnDisable() {
		_settingMenu?.OnButtonClick.RemoveListener(PlayButtonClickSound);
	}

	private void PlayButtonClickSound() {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
	}

	// Button Functions

	public void ChangeMasterVolume() {
		if (!AudioManager.Instance.InstanceIsPlaying(_masterTestInstance)) {
			_masterTestInstance.start();
		}
		AudioManager.Instance.SetMasterVolume(_masterVolumeSlider.value);
	}

	public void ChangeGameVolume() {
		if (!AudioManager.Instance.InstanceIsPlaying(_gameTestInstance)) {
			_gameTestInstance.start();
		}
		AudioManager.Instance.SetGameVolume(_gameVolumeSlider.value);
	}

	public void ChangeMusicVolume() {
		if (!AudioManager.Instance.InstanceIsPlaying(_musicTestInstance)) {
			_musicTestInstance.start();
		}
		AudioManager.Instance.SetMusicVolume(_musicVolumeSlider.value);
	}

	private void StopAllSounds() {
		_masterTestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		_gameTestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		_musicTestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}
}
