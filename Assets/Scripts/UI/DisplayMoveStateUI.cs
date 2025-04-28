using TMPro;
using UnityEngine;

public class DisplayMoveStateUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _textBox;
    [SerializeField] private MyCharacterController _characterController;

    private void Update() {
        _textBox.text = _characterController._state.ToString();
    }
}
