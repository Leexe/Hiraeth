using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandleDisplay : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private float _mult;

	public void OnValueChanged(float value) {
		value *= _mult;
		_text.text = value.ToString();
	}
}
