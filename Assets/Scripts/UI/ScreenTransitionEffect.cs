using UnityEngine;
using UnityEngine.Events;

public class ScreenTransitionEffect : MonoBehaviour {
	[SerializeField] private Animator _animator;

	private int _onDeathName;
	private int _onStartName;
	private int _onLevelBrowserSelectName;
	private int _onBackToMainMenuSelectName;

	[HideInInspector] public UnityEvent OnDeathTransitionFinish;
	[HideInInspector] public UnityEvent OnStartTransitionFinish;
	[HideInInspector] public UnityEvent OnLevelBrowserSelectTransitionFinish;
	[HideInInspector] public UnityEvent OnBackToMainMenuTransitionFinish;

	private void Start() {
		// Get hash codes for animation events
		_onDeathName = Animator.StringToHash("OnDeath");
		_onStartName = Animator.StringToHash("OnStart");
		_onLevelBrowserSelectName = Animator.StringToHash("OnLevelBrowserSelect");
		_onBackToMainMenuSelectName = Animator.StringToHash("OnBackToMainMenu");
	}

	// Player Reset on Level Transition

	public void PlayResetTransition() {
		if (_animator == null) {
			Debug.Log("Help");
			_animator = GetComponent<Animator>();
		}
		_animator.SetTrigger(_onDeathName);
	}

	public void DeathTransitionFinish() {
		OnDeathTransitionFinish?.Invoke();
	}

	// Back to Main Menu Transition

	public void PlayBackToMainMenuTransition() {
		_animator.SetTrigger(_onBackToMainMenuSelectName);
	}

	public void BackToMainMenuTransitionFinish() {
		OnBackToMainMenuTransitionFinish?.Invoke();
	}

	// Level Start Transition

	public void PlayLevelStartTransition() {
		_animator.SetTrigger(_onStartName);
	}

	public void LevelStartTransitionFinish() {
		OnStartTransitionFinish?.Invoke();
	}

	// Selecting a Level on the Level Browser Transtion

	public void PlayLevelBrowserSelectTransition() {
		_animator.SetTrigger(_onLevelBrowserSelectName);
	}

	public void LevelBrowserSelectTransitionFinish() {
		OnLevelBrowserSelectTransitionFinish?.Invoke();
	}
}
