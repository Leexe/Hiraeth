using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager> {
	[Header("References")]
	public GameObject _characterGameObject;
	public GameObject PlayerGameObject;
	[HideInInspector] public ShootingSystem ShootingSystemRef;
	[HideInInspector] public MyCharacterController MyCharacterControllerRef;
	[HideInInspector] public StaminaSystem _staminaSystem;
	public HandAnimations HandAnimationsRef;
	private HealthManager _healthManager;

	[Header("UI References")]
	[SerializeField] private PauseMenu _pauseMenu;

	// Events
	[HideInInspector] public UnityEvent OnGameLose;
	[HideInInspector] public UnityEvent OnGameWin;
	[HideInInspector] public UnityEvent OnTogglePauseMenu;
	[HideInInspector] public UnityEvent OnResume;
	[HideInInspector] public UnityEvent OnPause;
	[HideInInspector] public UnityEvent OnReset;
	[HideInInspector] public UnityEvent OnTimerStart;
	[HideInInspector] public UnityEvent<bool> OnTimerTypeChange;
	[HideInInspector] public UnityEvent<int> OnEnemyDeath;

	public float TimeOnLevel { get; private set; }
	public float HighScoreOnLevel { get; private set; }
	public bool TimerIsCounting { get; private set; }
	private bool _gameIsOver;
	private bool _gamePaused = false;

	override protected void Awake() {
		base.Awake();

		// Get References
		_staminaSystem = PlayerGameObject.GetComponent<StaminaSystem>();
		ShootingSystemRef = PlayerGameObject.GetComponent<ShootingSystem>();
		_healthManager = _characterGameObject.GetComponent<HealthManager>();
		MyCharacterControllerRef = _characterGameObject.GetComponent<MyCharacterController>();
	}

	private void Start() {
		// Get the High Score of the Current Level
		GetHighScoreFromFile();

		// Deal with game time
		TimeOnLevel = 0f;
		TimerIsCounting = false;

		// Lock Cursor
		LockCursor();

		// Add Event Listeners
		OnTogglePauseMenu.AddListener(ToggleEscapeMenu);
		OnReset.AddListener(Restart);
		OnTimerStart.AddListener(StartTime);
		OnEnemyDeath.AddListener(AddPlayerStamina);
		_healthManager.OnDeath.AddListener(LoseGame);
	}

	private void HandleGameTime() {
		if (TimerIsCounting) {
			TimeOnLevel += Time.deltaTime;
		}
	}

	private void Update() {
		HandleGameTime();
	}

	// Escape

	private void ToggleEscapeMenu() {
		_gamePaused = !_gamePaused;
		if (_gamePaused) {
			UnlockCursor();
			OnPause?.Invoke();
			PauseGameTime();
		}
		else {
			if (!_gameIsOver) {
				LockCursor();
			}
			OnResume?.Invoke();
			UnpauseGameTime();
		}
	}

	// Game States

	public void StartTime() {
		TimerIsCounting = true;
	}

	public void LoseGame() {
		Endgame();
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Lose_sfx, PlayerGameObject.transform.position);
		OnGameLose?.Invoke();
		LevelManger.Instance?.StartRestartScene();
	}

	public void WinGame() {
		Endgame();
		if (TimeOnLevel < HighScoreOnLevel) {
			HighScoreOnLevel = TimeOnLevel;
			PlayerPrefs.SetFloat(LevelManger.Instance.GetCurrLevelHighScoreName(), TimeOnLevel);
		}
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.LevelComplete_sfx, PlayerGameObject.transform.position);
		OnGameWin?.Invoke();
	}

	public void Restart() {
		UnpauseGameTime();
		LevelManger.Instance.StartRestartScene();
	}

	// Misc.

	public void AddPlayerStamina(int staminaGiven) {
		_staminaSystem.AddStaminaCharges(staminaGiven);
	}

	private void Endgame() {
		_gameIsOver = true;
		TimerIsCounting = false;
		UnlockCursor();
	}

	private void GetHighScoreFromFile() {
		HighScoreOnLevel = PlayerPrefs.GetFloat(LevelManger.Instance.GetCurrLevelHighScoreName(), float.MaxValue);
	}

	public void QuitGame() {
		UnpauseGameTime();
		LevelManger.Instance.StartLoadMainMenu();
	}

	private void LockCursor() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void UnlockCursor() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void PauseGameTime() {
		Time.timeScale = 0f;
	}

	private void UnpauseGameTime() {
		Time.timeScale = 1f;
	}
}