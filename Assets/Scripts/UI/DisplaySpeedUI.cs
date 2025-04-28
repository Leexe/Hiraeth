using TMPro;
using UnityEngine;

public class DisplaySpeedUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _textBox;
    [SerializeField] private MyCharacterController _characterController;

    private void Update() {
        float horizontalVelocity = new Vector3(_characterController.CurrentVelocity.x, 0, _characterController.CurrentVelocity.z).magnitude;
        _textBox.text = "Speed: " + horizontalVelocity.ToString("0.0");
    }
}
