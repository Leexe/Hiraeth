using UnityEngine;

public class MainMenuSound : MonoBehaviour {
	[SerializeField] private MainMenuUI _mainMenuUI;

	private void Start() {
		// Event Listeners
		_mainMenuUI.OnButtonClick.AddListener(PlayButtonSound);
	}

	private void PlayStartSound() {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GameStartUp_sfx, Vector3.zero);
	}

	private void PlayButtonSound() {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
	}

	public void PlayButtonHover() {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseHover_sfx, Vector3.zero);
	}
}
