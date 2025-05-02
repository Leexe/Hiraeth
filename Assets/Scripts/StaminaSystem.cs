using UnityEngine;
using UnityEngine.Events;

public class StaminaSystem : MonoBehaviour {
    [SerializeField] private int maxStaminaCharges = 3;
    [SerializeField] private float staminaBaseRechargeRate= 2f;
    private float rechargeTimer = 0f;
    private int staminaCharges = 0;

    // int -> Charges, float -> rechargePercentage
    public UnityEvent<int, float> StaminaRecharging;

    private void Start() {
        staminaCharges = maxStaminaCharges;
    }

    private void Update() {
        if (staminaCharges < maxStaminaCharges) {
            if (rechargeTimer >= staminaBaseRechargeRate) {
                staminaCharges++;
                rechargeTimer = 0f;
            }
            rechargeTimer += Time.deltaTime;
            StaminaRecharging?.Invoke(staminaCharges, rechargeTimer / staminaBaseRechargeRate);
        }
        else {
            rechargeTimer = 0f;
        }
    }

    // Adds stamina chrages
    public void AddStaminaCharges(int charges) {
        staminaCharges += charges;
        if (staminaCharges > maxStaminaCharges) {
            staminaCharges = maxStaminaCharges;
        }
    }

    // Returns a boolean indicating if the a stamina charge can be consumed, does not consume stamina charages
    public bool CanConsumeStamina() {
        if (staminaCharges > 0) {
            return true;
        }
        else {
            return false;
        }
    }

    // Returns a boolean indicating if the a stamina charge can be consumed, does consume stamina charages
    public bool ConsumeStamina() {
        if (staminaCharges > 0) {
            staminaCharges--;
            return true;
        }
        else {
            Debug.LogWarning("Attempted to consume stamina but no stamina chrages");
            return false;
        }
    }

    // Getters

    public float GetMaxStamina => maxStaminaCharges;

    public int GetStaminaCharges => staminaCharges;
}
