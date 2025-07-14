using UnityEngine;
using UnityEngine.Events;

public class MainMenuUI : Singleton<MainMenuUI> {
	[Header("References")]
	[SerializeField] private SettingsUI _settings;
	[SerializeField] private LevelBrowserUI _levelBrowser;
	[SerializeField] private Animator _animator;

	// Events
	[HideInInspector] public UnityEvent OnButtonClick;

	private int _enterLevelBrowserName;
	private int _enterCreditsName;

	private bool _buttonsActive = true;

	private void Start() {
		_enterLevelBrowserName = Animator.StringToHash("EnterLevelBrowser");
		_enterCreditsName = Animator.StringToHash("EnterCredits");
	}

	public void OpenSettings() {
		if (_buttonsActive) {
			OnButtonClick?.Invoke();
			_settings.EnableSettings();
		}
	}

	public void OpenLevelBrowser() {
		if (_buttonsActive) {
			OnButtonClick?.Invoke();
			_animator.SetTrigger(_enterLevelBrowserName);
		}
	}

	public void OpenCredits() {
		if (_buttonsActive && !_settings.IsOpened) {
			OnButtonClick?.Invoke();
			_animator.SetTrigger(_enterCreditsName);
		}
	}

	public void QuitGame() {
		if (_buttonsActive) {
			OnButtonClick?.Invoke();
			Application.Quit();
		}
	}

	private void SetButtonsActive() {
		_buttonsActive = true;
	}

	private void SetButtonsInactive() {
		_buttonsActive = false;
	}
}
