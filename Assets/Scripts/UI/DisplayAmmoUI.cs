using TMPro;
using UnityEngine;

public class DisplayAmmoUI : UI {
	[SerializeField] private TextMeshProUGUI _textBox;
	private ShootingSystem _shootingSystem;

	private float _maxAmmo;

	private void Start() {
		_shootingSystem = GameManager.Instance.ShootingSystemRef;
		_maxAmmo = _shootingSystem.GetMaxAmmo;
		_textBox.text = _maxAmmo.ToString();
		GameManager.Instance.OnGameLose.AddListener(FadeOut);
		GameManager.Instance.OnGameWin.AddListener(FadeOut);
		GameManager.Instance.OnTimerStart.AddListener(FadeIn);
	}

	private void Update() {
		if (_shootingSystem.IsReloading) {
			_textBox.text = "0";
		}
		else {
			_textBox.text = _shootingSystem.GetAmmo.ToString();
		}
	}

	private void FadeIn() {
		StartFadeInText(_textBox);
	}

	private void FadeOut() {
		StartFadeOutText(_textBox);
	}
}
