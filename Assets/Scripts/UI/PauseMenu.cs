using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	[Header("References")]
	[SerializeField] private GameObject _pauseScreen;
	[SerializeField] private SettingsUI _settingUI;
	[SerializeField] private HighScoreDisplayUI _highScoreDisplay;
	[SerializeField] private Image _background;

	[Header("Background Opacity")]
	[SerializeField] private float _startBackgroundOpacity = 0f;
	[SerializeField] private float _endBackgroundOpacity = 0.6f;
	[SerializeField] private float _backgroundOpacityDuration = 1f;

	[Header("Pause Menu Transition Effects")]
	[SerializeField] private Vector3 _startScale = new Vector3(0.1f, 0.1f, 0.1f);
	[SerializeField] private Vector3 _endScale = new Vector3(1f, 1f, 1f);
	[SerializeField] private Vector3 _startPosition = new Vector3(433, -264, 0f);
	[SerializeField] private Vector3 _endPosition = new Vector3(0f, 0f, 0f);
	[SerializeField] private float _transitionDuration = 1f;

	private Color _initalColor;
	private Tween _disablePauseMenuDelay;
	private Tween _PauseMenuBackgroundTween;
	private Tween _PauseMenuScaleTween;
	private Tween _PauseMenuPositionTween;

	// Events
	[HideInInspector] public UnityEvent OnButtonClick;

	private void Start() {
		_background = _background.GetComponent<Image>();
		_initalColor = _background.color;

		// Set Default Menu Value
		_pauseScreen.SetActive(false);
		_background.color = new Color(_initalColor.r, _initalColor.b, _initalColor.g, _startBackgroundOpacity);
		_pauseScreen.transform.localScale = _startScale;
		_pauseScreen.transform.localPosition = _startPosition;
		GameManager.Instance.OnPause.AddListener(EnablePauseMenu);
		GameManager.Instance.OnResume.AddListener(DisablePauseMenu);
	}

	private void OnDisable() {
		// Disable Tweens
		if (_PauseMenuBackgroundTween.isAlive) {
			_PauseMenuBackgroundTween.Complete();
		}
		if (_PauseMenuScaleTween.isAlive) {
			_PauseMenuScaleTween.Complete();
		}
		if (_PauseMenuPositionTween.isAlive) {
			_PauseMenuPositionTween.Complete();
		}
	}

	private void OnDestroy() {
		GameManager.Instance?.OnPause.RemoveListener(EnablePauseMenu);
		GameManager.Instance?.OnResume.RemoveListener(DisablePauseMenu);
	}

	public void EnablePauseMenu() {
		// Stop the Tween Pause Delay
		if (_disablePauseMenuDelay.isAlive) {
			_disablePauseMenuDelay.Stop();
		}

		// If the current level isn't the main menu, display the high score
		if (LevelManger.Instance.CurrentLevel != LevelManger.Level.mainMenu && LevelManger.Instance.IsLevelCompleted(LevelManger.Instance.CurrentLevel)) {
			_highScoreDisplay.EnableDisplay();
		}

 		// Activate Screen
		_pauseScreen.SetActive(true);

		// Background Opacity Lerp
		_PauseMenuBackgroundTween = Tween.Custom(_background.color.a, _endBackgroundOpacity, _backgroundOpacityDuration, onValueChange: newOpacity => _background.color = new Color(_initalColor.r, _initalColor.b, _initalColor.g, newOpacity), Ease.InSine, useUnscaledTime: true);

		// Menu Screen Scale Lerp
		_PauseMenuScaleTween = Tween.Custom(_pauseScreen.transform.localScale, _endScale, _transitionDuration, onValueChange: newScale => _pauseScreen.transform.localScale = newScale, Ease.InSine, useUnscaledTime: true);

		// Menu Screen Position Lerp
		_PauseMenuPositionTween = Tween.Custom(_pauseScreen.transform.localPosition, _endPosition, _transitionDuration, onValueChange: newPosition => _pauseScreen.transform.localPosition = newPosition, Ease.InSine, useUnscaledTime: true);
	}

	public void DisablePauseMenu() {
		// If the current level isn't the main menu, display the high score
		if (LevelManger.Instance.CurrentLevel != LevelManger.Level.mainMenu && LevelManger.Instance.IsLevelCompleted(LevelManger.Instance.CurrentLevel)) {
			_highScoreDisplay.DisableDisplay();
		}

		// Deactivate Pause Screen After Some Time
		_disablePauseMenuDelay = Tween.Delay(this, duration: _transitionDuration, target => _pauseScreen.SetActive(false), useUnscaledTime: true);

		// Background Opacity Lerp
		_PauseMenuBackgroundTween = Tween.Custom(_background.color.a, _startBackgroundOpacity, _backgroundOpacityDuration, onValueChange: newOpacity => _background.color = new Color(_initalColor.r, _initalColor.b, _initalColor.g, newOpacity), Ease.OutSine, useUnscaledTime: true);

		// Menu Screen Scale Lerp
		_PauseMenuScaleTween = Tween.Custom(_pauseScreen.transform.localScale, _startScale, _transitionDuration, onValueChange: newScale => _pauseScreen.transform.localScale = newScale, Ease.InSine, useUnscaledTime: true);

		// Menu Screen Position Lerp
		_PauseMenuPositionTween = Tween.Custom(_pauseScreen.transform.localPosition, _startPosition, _transitionDuration, onValueChange: newPosition => _pauseScreen.transform.localPosition = newPosition, Ease.InSine, useUnscaledTime: true);

		// Disable Settings
		_settingUI.DisableSettings();
	}

	public void ResumeGame() {
		OnButtonClick?.Invoke();
		GameManager.Instance.OnTogglePauseMenu?.Invoke();
	}

	public void EnableSettingsMenu() {
		OnButtonClick?.Invoke();
		_settingUI.EnableSettings();
	}

	public void ResetGame() {
		OnButtonClick?.Invoke();
		GameManager.Instance.OnReset?.Invoke();
	}

	public void QuitGame() {
		OnButtonClick?.Invoke();
		GameManager.Instance.QuitGame();
	}
}
