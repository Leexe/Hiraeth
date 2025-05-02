using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class StaminaSystem : MonoBehaviour {
    [SerializeField] private int maxStaminaCharges = 3;
    [SerializeField] private float staminaBaseRechargeRate= 2f;
    private float rechargeTimer = 0f;
    private int staminaChrages;

    private void Start() {
        staminaChrages = maxStaminaCharges;
    }

    private void Update() {
        if (staminaChrages < maxStaminaCharges) {
            if (rechargeTimer >= staminaBaseRechargeRate) {
                staminaChrages++;
                rechargeTimer = 0f;
            }
            rechargeTimer += Time.deltaTime;
        }
        else {
            rechargeTimer = 0f;
        }
    }

    public void AddStaminaCharges(int charges) {
        staminaChrages += charges;
        if (staminaChrages > maxStaminaCharges) {
            staminaChrages = maxStaminaCharges;
        }
    }

    public bool CanConsumeStamina() {
        if (staminaChrages > 0) {
            return true;
        }
        else {
            return false;
        }
    }

    public bool ConsumeStamina() {
        if (staminaChrages > 0) {
            staminaChrages--;
            return true;
        }
        else {
            Debug.LogWarning("Attempted to consume stamina but no stamina chrages");
            return false;
        }
    }

    public float GetRechargeTimer => rechargeTimer;

    public int GetStaminaCharges => staminaChrages;
}
