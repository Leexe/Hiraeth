using TMPro;
using UnityEngine;

public class DisplaySpeedUI : UI {
	[SerializeField] private TextMeshProUGUI _textBox;
	private MyCharacterController _characterController;

	private void Start() {
		_characterController = GameManager.Instance.MyCharacterControllerRef;
		GameManager.Instance.OnTimerStart.AddListener(FadeIn);
		GameManager.Instance.OnGameLose.AddListener(FadeOut);
		GameManager.Instance.OnGameWin.AddListener(FadeOut);
	}

	private void Update() {
		float horizontalVelocity = _characterController.CurrentHorVelocity.magnitude;
		_textBox.text = "Speed: " + horizontalVelocity.ToString("0.0");
	}

	private void FadeIn() {
		StartFadeInText(_textBox);
	}

	private void FadeOut() {
		StartFadeOutText(_textBox);
	}
}
