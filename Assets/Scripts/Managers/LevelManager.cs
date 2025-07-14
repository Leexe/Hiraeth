using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManger : PersistantSingleton<LevelManger> {
	public enum Level {
		mainMenu = 0,
		tutorial = 1,
		level1 = 2,
		level2 = 3,
		level3 = 4,
	}

	public enum Medal {
		none = 0,
		bronze = 1,
		silver = 2,
		gold = 3,
	}

	[System.Serializable]
	public struct Medals {
		public Medals(float bronzeTime = float.MaxValue, float silverTime = float.MaxValue) {
			this.bronzeTime = bronzeTime;
			this.silverTime = silverTime;
		}

		public float bronzeTime;
		public float silverTime;
	}

	[Header("Reference")]
	[SerializeField] private ScreenTransitionEffect _screenTransitionEffect;

	[Header("Badge Time Requirements")]
	[SerializeField] private List<Medals> _badgeTimeRequirements;

	private Level _levelToLoad = Level.tutorial;
	[HideInInspector] public Level CurrentLevel;
	[SerializeField] private Level lastLevel = Level.level1;
	public bool IsLastLevel => CurrentLevel == lastLevel;

	// Events
	[HideInInspector] public UnityEvent OnLevelStartTransitionFinish;
	[HideInInspector] public UnityEvent<Level> OnNewSceneLoaded;

	override protected void Awake() {
		base.Awake();
		CurrentLevel = (Level)SceneManager.GetActiveScene().buildIndex;
	}

	private void Start() {
		// Event Listeners
		_screenTransitionEffect.OnDeathTransitionFinish.AddListener(RestartScene);
		_screenTransitionEffect.OnLevelBrowserSelectTransitionFinish.AddListener(LoadLevel);
		_screenTransitionEffect.OnStartTransitionFinish.AddListener(TriggerStartLevelTransitionEvent);
		_screenTransitionEffect.OnBackToMainMenuTransitionFinish.AddListener(LoadMainMenu);
		SceneManager.sceneLoaded += OnSceneLoaded;

		// If the scene is not the main menu, start the level load animation
		if (SceneManager.GetActiveScene().buildIndex > 0) {
			_screenTransitionEffect.PlayLevelStartTransition();
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		// If the scene is not the main menu, start the level load animation
		if (scene.buildIndex > 0) {
			_screenTransitionEffect.PlayLevelStartTransition();
		}

		// If the next scene is not the same as the old
		if (CurrentLevel != (Level)scene.buildIndex) {
			OnNewSceneLoaded?.Invoke((Level)scene.buildIndex);
		}
		CurrentLevel = (Level)scene.buildIndex;
	}

	public void StartRestartScene() {
		_screenTransitionEffect.PlayResetTransition();
	}

	public void StartLoadMainMenu() {
		_screenTransitionEffect.PlayBackToMainMenuTransition();
	}

	public void StartLoadLevel(Level level) {
		_levelToLoad = level;
		_screenTransitionEffect.PlayLevelBrowserSelectTransition();
	}

	public void StartLoadNextLevel() {
		if ((int)CurrentLevel == (int)Level.level3) {
			Debug.LogError("Tried to load next level on the last level");
			return;
		}
		_levelToLoad = (Level)((int)CurrentLevel + 1);
		_screenTransitionEffect.PlayLevelBrowserSelectTransition();
	}

	private void RestartScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void LoadMainMenu() {
		SceneManager.LoadScene(0);
	}

	private void LoadLevel() {
		SceneManager.LoadScene((int)_levelToLoad);
	}

	private void TriggerStartLevelTransitionEvent() {
		OnLevelStartTransitionFinish?.Invoke();
	}

	public string GetCurrLevelHighScoreName() {
		return GetSceneAsString(CurrentLevel) + "_Time";
	}

	public string GetLevelHighScoreName(Level level) {
		return GetSceneAsString(level) + "_Time";
	}

	public float GetHighScoreFromLevel(Level level) {
		return PlayerPrefs.GetFloat(GetLevelHighScoreName(level), float.MaxValue);
	}

	public bool IsLevelCompleted(Level level) {
		return GetHighScoreFromLevel(level) != float.MaxValue;
	}

	public Medal GetMedal(Level level, float score) {
		if (score == float.MaxValue) {
			return Medal.none;
		}
		else if (score >= _badgeTimeRequirements[(int)level - 1].bronzeTime) {
			return Medal.bronze;
		}
		else if (score >= _badgeTimeRequirements[(int)level - 1].silverTime) {
			return Medal.silver;
		}
		else {
			return Medal.gold;
		}
	}

	public Medal GetMedalWithHighScore(Level level) {
		float highScore = GetHighScoreFromLevel(level);
		return GetMedal(level, highScore);
	}


	public string GetSceneAsString(Level level) {
		if (level == Level.mainMenu) {
			return "mainMenu";
		}
		else if (level == Level.tutorial) {
			return "tutorial";
		}
		else if (level == Level.level1) {
			return "level1";
		}
		else if (level == Level.level2) {
			return "level2";
		}
		else {
			return "level3";
		}
	}
}
