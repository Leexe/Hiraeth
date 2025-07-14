using UnityEngine;

public class WinScreenUI : MonoBehaviour {
	[Header("References")]
	[SerializeField] private Animator _animator;

	private int _onWinName;
	private int _onWinLastLevelName;

	private void Start() {
		_onWinName = Animator.StringToHash("OnWin");
		_onWinLastLevelName = Animator.StringToHash("OnWinLastLevel");

		if (LevelManger.Instance.IsLastLevel) {
			GameManager.Instance.OnGameWin.AddListener(DisplayWinScreenWithoutNextLevel);
		}
		else {
			GameManager.Instance.OnGameWin.AddListener(DisplayWinScreen);
		}
	}

	private void OnDestroy() {
		if (LevelManger.Instance && LevelManger.Instance.IsLastLevel) {
			GameManager.Instance?.OnGameWin.RemoveListener(DisplayWinScreenWithoutNextLevel);
		}
		else {
			GameManager.Instance?.OnGameWin.RemoveListener(DisplayWinScreen);
		}
	}

	private void DisplayWinScreen() {
		_animator.SetTrigger(_onWinName);
	}

	private void DisplayWinScreenWithoutNextLevel() {
		_animator.SetTrigger(_onWinLastLevelName);
	}

	public void OnMainMenuPressed() {
		LevelManger.Instance.StartLoadMainMenu();
	}

	public void OnNextLevelPressed() {
		LevelManger.Instance.StartLoadNextLevel();
	}

	public void OnResetPressed() {
		GameManager.Instance.Restart();
	}
}
