using TMPro;
using UnityEngine;

public class DisplaySpeedUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _textBox;
    [SerializeField] private MyCharacterController _characterController;

    private void Update() {
        float horizontalVelocity = _characterController.CurrentHorVelocity.magnitude;
        _textBox.text = "Speed: " + horizontalVelocity.ToString("0.0");
    }
}
