using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image staminaSlider;
    [SerializeField] private StaminaSystem StaminaSystem;

    [Header("Values")]
    [Tooltip("How long the tooltip can be inactive before it disapears")]
    [SerializeField] private StaminaSystem disapearAfterTime;
    
    private float maxStamina;
    private float oneStaminaFillAmount;
    
    private void Start() {
        maxStamina = StaminaSystem.GetMaxStamina;
        oneStaminaFillAmount = 1 / maxStamina;
        StaminaSystem.StaminaRecharging.AddListener(UpdateStaminaUI);
        staminaSlider.fillAmount = 1f;
    }

    private void OnDisable() {
        StaminaSystem.StaminaRecharging.RemoveListener(UpdateStaminaUI);
    }

    private void Update() {

    }

    private void UpdateStaminaUI(int stamina, float rechargePercentage) {
        staminaSlider.fillAmount = Mathf.Clamp(stamina * oneStaminaFillAmount + oneStaminaFillAmount * rechargePercentage, 0f, 1f);
    }
}
