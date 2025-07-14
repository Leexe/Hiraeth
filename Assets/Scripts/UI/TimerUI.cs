using System;
using TMPro;
using UnityEngine;

public class TimerUI : UI {
	[SerializeField] private TextMeshProUGUI _textBox;
	[SerializeField] private bool _toggleMilliseconds = false;

	private void Start() {
		_toggleMilliseconds = PlayerPrefs.GetInt("TimerMiliseconds", 0) == 1;
		GameManager.Instance.OnTimerTypeChange.AddListener(ToggleTimerAndUpdateTime);
		GameManager.Instance.OnTimerStart.AddListener(FadeIn);
		GameManager.Instance.OnGameWin.AddListener(FadeOut);
	}

	private void OnDestroy() {
		GameManager.Instance?.OnTimerTypeChange.RemoveListener(ToggleTimerAndUpdateTime);
	}

	private void Update() {
		if (GameManager.Instance.TimerIsCounting) {
			UpdateTime();
		}
	}

	private void ToggleTimerAndUpdateTime(bool timerToggleMilliseconds) {
		_toggleMilliseconds = timerToggleMilliseconds;
		UpdateTime();
	}

	private void UpdateTime() {
		TimeSpan t = TimeSpan.FromSeconds(GameManager.Instance.TimeOnLevel);
		if (_toggleMilliseconds) {
			_textBox.text = t.ToString(@"mm\:ss\:ff");
		}
		else {
			_textBox.text = t.ToString(@"mm\:ss");
		}
	}

	private void FadeIn() {
		StartFadeInText(_textBox);
	}

	private void FadeOut() {
		StartFadeOutText(_textBox);
	}
}
