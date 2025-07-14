using PrimeTween;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour {
	protected void DisableUI() {
		gameObject.SetActive(false);
	}

	protected void StartFadeInText(TextMeshProUGUI text) {
		Tween.Custom(text.color, Color.white, 1.5f, newVal => text.color = newVal, Ease.InOutSine);
	}

	protected void StartFadeInText(TextMeshProUGUI text, Color endColor, float duration = 0.7f) {
		Tween.Custom(text.color, endColor, duration, newVal => text.color = newVal, Ease.InOutSine);
	}

	protected void StartFadeOutText(TextMeshProUGUI text, float duration = 0.7f) {
		Tween.Custom(text.color, new Color(0f, 0f, 0f, 0f), duration, newVal => text.color = newVal, Ease.InOutSine);
	}
}