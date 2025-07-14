using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HighScoreTextUI : UI {
	[Header("Header")]
	[SerializeField] private TextMeshProUGUI _text;

	[Header("Required Data For Main Menu")]
	[SerializeField] private bool _isInMainMenu = false;
	[ShowIf("_isInMainMenu")]
	[SerializeField] private LevelManger.Level _level;

	private void OnEnable() {
		if (LevelManger.Instance.CurrentLevel == LevelManger.Level.mainMenu) {
			float time = LevelManger.Instance.GetHighScoreFromLevel(_level);

			// If the level has not been completed yet, don't display time
			if (time == float.MaxValue) {
				gameObject.SetActive(false);
				return;
			}

			TimeSpan timeSpan = TimeSpan.FromSeconds(time);
			if (time > 599f) {
				_text.text = timeSpan.ToString(@"mm\:ss\.ff");
			}
			else {
				_text.text = timeSpan.ToString(@"m\:ss\.ff");
			}

		}
		else {
			float time = GameManager.Instance.HighScoreOnLevel;

			// If the level has not been completed yet, don't display time
			if (time == float.MaxValue) {
				gameObject.SetActive(false);
				return;
			}

			TimeSpan timeSpan = TimeSpan.FromSeconds(time);
			if (time > 599f) {
				_text.text = timeSpan.ToString(@"mm\:ss\.ff");
			}
			else {
				_text.text = timeSpan.ToString(@"m\:ss\.ff");
			}
		}
	}
}
