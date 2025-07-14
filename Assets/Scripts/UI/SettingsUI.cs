using UnityEngine;
using UnityEngine.Events;
using PrimeTween;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {
	[Header("References")]
	[SerializeField] private GameObject _settingsMenu;
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private Slider _cameraSensSlider;
	[SerializeField] private Toggle _timerToggle;

	[Header("Background")]
	[SerializeField] private Image _backgroundImage;
	[SerializeField] private Sprite _videoSettingsSprite;
	[SerializeField] private Sprite _soundSettingsSprite;
	[SerializeField] private Sprite _controlsSettingsSprite;

	[Header("Setting Pages")]
	[SerializeField] private GameObject _videoPage;
	[SerializeField] private GameObject _soundPage;
	[SerializeField] private GameObject _controlsPage;

	[Header("Settings Transition Effects")]
	[SerializeField] private Vector3 _startScale = new Vector3(0.1f, 0.1f, 0.1f);
	[SerializeField] private Vector3 _endScale = new Vector3(1f, 1f, 1f);
	[SerializeField] private Vector3 _startPosition = new Vector3(433, -264, 0f);
	[SerializeField] private Vector3 _endPosition = new Vector3(0f, 0f, 0f);
	[SerializeField] private float _transitionDuration = 1f;

	// Tweens
	private Tween _SettingScaleTween;
	private Tween _SettingPositionTween;

	// Events
	[HideInInspector] public UnityEvent OnButtonClick;

	public bool IsOpened { get; private set; }
	private bool _timerToggleMilliseconds;
	private float _cameraSensitivity;

	private void Start() {
		// Set Default Menu Values
		_settingsMenu.SetActive(false);
		_settingsMenu.transform.localScale = _startScale;
		_settingsMenu.transform.localPosition = _startPosition;
		IsOpened = false;

		// Get Defualt Values from File
		_cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
		_timerToggleMilliseconds = PlayerPrefs.GetInt("TimerMiliseconds", 0) == 1;
	}

	private void OnDestroy() {
		if (_SettingScaleTween.isAlive) {
			_SettingScaleTween.Complete();
		}

		if (_SettingPositionTween.isAlive) {
			_SettingPositionTween.Complete();
		}
	}

	public void EnableSettings() {
		_settingsMenu.SetActive(true);
		IsOpened = true;

		// Settings Screen Scale Lerp
		_SettingScaleTween = Tween.Custom(_settingsMenu.transform.localScale, _endScale, _transitionDuration, onValueChange: newScale => _settingsMenu.transform.localScale = newScale, Ease.InSine, useUnscaledTime: true);

		// Settings Screen Position Lerp
		_SettingPositionTween = Tween.Custom(_settingsMenu.transform.localPosition, _endPosition, _transitionDuration, onValueChange: newPosition => _settingsMenu.transform.localPosition = newPosition, Ease.InSine, useUnscaledTime: true);

		// Default Opened Page
		OpenVideoPage();

		// Set Default Values
		_timerToggle.isOn = _timerToggleMilliseconds;
		_cameraSensSlider.value = _cameraSensitivity * 20f;
	}

	public void DisableSettings() {
		IsOpened = false;

		// Disable Screen Object After Some Time
		DisableSettingsScreenGameObject();

		// Save Player Prefs To File
		SavePlayerPref();

		// Apply Sensitivity Settings
		CameraManager.Instance?.ChangeSensitivity(_cameraSensitivity);

		// Menu Screen Scale Lerp
		_SettingScaleTween = Tween.Custom(_settingsMenu.transform.localScale, _startScale, _transitionDuration, onValueChange: newScale => _settingsMenu.transform.localScale = newScale, Ease.InSine, useUnscaledTime: true);

		// Menu Screen Position Lerp
		_SettingPositionTween = Tween.Custom(_settingsMenu.transform.localPosition, _startPosition, _transitionDuration, onValueChange: newPosition => _settingsMenu.transform.localPosition = newPosition, Ease.InSine, useUnscaledTime: true);
	}

	private void SavePlayerPref() {
		AudioManager.Instance.SaveAudioPref();
		PlayerPrefs.SetInt("TimerMiliseconds", _timerToggleMilliseconds ? 1 : 0);
		PlayerPrefs.SetFloat("CameraSensitivity", _cameraSensitivity);
	}

	// Back Button Was Pressed, Close the Setttings Menu
	public void OnBackButtonPress() {
		OnButtonClick?.Invoke();
		DisableSettings();
	}

	// Switch to video settings page
	public void OpenVideoPage() {
		OnButtonClick?.Invoke();
		_backgroundImage.sprite = _videoSettingsSprite;
		_videoPage.SetActive(true);
		_soundPage.SetActive(false);
		_controlsPage.SetActive(false);
		_scrollRect.content = _videoPage.GetComponent<RectTransform>();
	}

	// Switch to sound settings page
	public void OpenSoundPage() {
		OnButtonClick?.Invoke();
		_backgroundImage.sprite = _soundSettingsSprite;
		_videoPage.SetActive(false);
		_soundPage.SetActive(true);
		_controlsPage.SetActive(false);
		_scrollRect.content = _soundPage.GetComponent<RectTransform>();
	}

	// Switch to controls settings page
	public void OpenControlPage() {
		OnButtonClick?.Invoke();
		_backgroundImage.sprite = _controlsSettingsSprite;
		_videoPage.SetActive(false);
		_soundPage.SetActive(false);
		_controlsPage.SetActive(true);
		_scrollRect.content = _controlsPage.GetComponent<RectTransform>();
	}

	// Disable the settings screen after it was closed for a delay
	private void DisableSettingsScreenGameObject() {
		Tween.Delay(this, duration: _transitionDuration, target => _settingsMenu.SetActive(false), useUnscaledTime: true);
	}

	// Invoke the timer toggle miliseconds event
	public void OnTimerMilisecondsButton() {
		_timerToggleMilliseconds = _timerToggle.isOn;
		GameManager.Instance?.OnTimerTypeChange?.Invoke(_timerToggleMilliseconds);
	}

	// Get the camera sensitivty value
	public void OnMouseSensSliderChange() {
		_cameraSensitivity = _cameraSensSlider.value * 0.05f;
	}
}