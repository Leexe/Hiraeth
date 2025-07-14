using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : UI {
	[Header("References")]
	[SerializeField] private Image _staminaSlider;
	private StaminaSystem _staminaSystem;

	[Header("Values")]
	[Tooltip("How long the tooltip can be inactive before it disapears")]
	[SerializeField] private float disapearAfterTime;

	private float maxStamina;
	private float oneStaminaFillAmount;

	private void Start() {
		_staminaSystem = GameManager.Instance._staminaSystem;
		maxStamina = _staminaSystem.GetMaxStamina;
		oneStaminaFillAmount = 1 / maxStamina;
		_staminaSlider.fillAmount = 1f;

		// Events
		_staminaSystem.OnStaminaRecharging.AddListener(UpdateStaminaUI);
		GameManager.Instance.OnGameLose.AddListener(DisableUI);
		GameManager.Instance.OnGameWin.AddListener(DisableUI);
	}

	private void OnDisable() {
		_staminaSystem.OnStaminaRecharging.RemoveListener(UpdateStaminaUI);
	}

	private void UpdateStaminaUI(int stamina, float rechargePercentage) {
		_staminaSlider.fillAmount = Mathf.Clamp(stamina * oneStaminaFillAmount + oneStaminaFillAmount * rechargePercentage, 0f, 1f);
	}
}
