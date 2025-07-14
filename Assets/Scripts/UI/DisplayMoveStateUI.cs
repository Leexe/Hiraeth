using TMPro;
using UnityEngine;

public class DisplayMoveStateUI : UI {
	[SerializeField] private TextMeshProUGUI _textBox;
	private MyCharacterController _characterController;

	private void Start() {
		_characterController = GameManager.Instance.MyCharacterControllerRef;
		GameManager.Instance.OnTimerStart.AddListener(FadeIn);
		GameManager.Instance.OnGameLose.AddListener(FadeOut);
		GameManager.Instance.OnGameWin.AddListener(FadeOut);
	}

	private void Update() {
		_textBox.text = _characterController._state.ToString();
	}

	private void FadeIn() {
		StartFadeInText(_textBox);
	}

	private void FadeOut() {
		StartFadeOutText(_textBox);
	}
}
