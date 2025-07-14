using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;

public class HighScoreDisplayUI : UI {
	[SerializeField] private RectTransform _rectTransform;

	public void EnableDisplay() {
		Tween.Custom(0, 1f, 0.5f, newVal => _rectTransform.localScale = new Vector3(newVal, newVal, newVal), Ease.InOutSine, useUnscaledTime: true);
	}

	public void DisableDisplay() {
		Tween.Custom(1f, 0f, 0.2f, newVal => _rectTransform.localScale = new Vector3(newVal, newVal, newVal), Ease.InOutSine, useUnscaledTime: true);
	}
}
