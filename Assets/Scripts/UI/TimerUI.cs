using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _textBox;

    private void Update() {
        if (GameManager.Instance.TimerIsCounting) {
            TimeSpan t = TimeSpan.FromSeconds(GameManager.Instance.TimeOnLevel);
            _textBox.text = t.ToString(@"mm\:ss");;
        }
    }
}
