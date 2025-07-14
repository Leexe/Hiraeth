using UnityEngine;

public class PauseMenuSounds : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PauseMenu _pauseMenu;

	private void Start() {
		GameManager.Instance.OnPause.AddListener(PlayPauseSound);
		GameManager.Instance.OnResume.AddListener(PlayResumeSound);
		_pauseMenu.OnButtonClick.AddListener(PlayButtonClickSound);
	}

	private void OnDisable() {
		GameManager.Instance?.OnPause.RemoveListener(PlayPauseSound);
		GameManager.Instance?.OnResume.RemoveListener(PlayResumeSound);
		_pauseMenu?.OnButtonClick.RemoveListener(PlayButtonClickSound);
	}

	private void PlayPauseSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Pause_sfx, GameManager.Instance._characterGameObject);
	}

	private void PlayResumeSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Unpause_sfx, GameManager.Instance._characterGameObject);
	}

	private void PlayButtonClickSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.PauseClick_sfx, GameManager.Instance._characterGameObject);
	}
}
