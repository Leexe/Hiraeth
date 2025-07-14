using UnityEngine;

public class LevelBrowserUI : MonoBehaviour {
	[SerializeField] private Animator _animator;

	private int _exitLevelBrowserName;
	private bool _levelSelected = false;

	private void Start() {
		_exitLevelBrowserName = Animator.StringToHash("ExitLevelBrowser");
	}

	public void OnTutorialButtonPress() {
		if (!_levelSelected) {
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
			LevelManger.Instance.StartLoadLevel(LevelManger.Level.tutorial);
			_levelSelected = true;
		}
	}

	public void OnFirstLevelButtonPress() {
		if (!_levelSelected) {
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
			LevelManger.Instance.StartLoadLevel(LevelManger.Level.level1);
			_levelSelected = true;
		}
	}

	public void OnSecondLevelButtonPress() {
		if (!_levelSelected) {
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
			LevelManger.Instance.StartLoadLevel(LevelManger.Level.level2);
			_levelSelected = true;
		}
	}

	public void OnThirdLevelButtonPress() {
		if (!_levelSelected) {
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
			LevelManger.Instance.StartLoadLevel(LevelManger.Level.level3);
			_levelSelected = true;
		}
	}

	public void OnBackButtonPress() {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PauseClick_sfx, Vector3.zero);
		_animator.SetTrigger(_exitLevelBrowserName);
	}
}
