using System;
using TMPro;
using UnityEngine;

public class CurrentTimeUI : UI {
	[Header("Header")]
	[SerializeField] private TextMeshProUGUI _text;

	private void OnEnable() {
		float time = GameManager.Instance.TimeOnLevel;
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		if (time > 599f) {
			Debug.Log(time);
			_text.text = timeSpan.ToString(@"mm\:ss\.ff");
		}
		else {
			_text.text = timeSpan.ToString(@"m\:ss\.ff");
		}
	}
}
