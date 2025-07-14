using System.Collections;
using PrimeTween;
using UnityEngine;

public class IconWaveEffectUI : MonoBehaviour {
	[Header("References")]
	[SerializeField] private RectTransform _rectTransform;
	private Vector3 _defaultRectPosition;

	[Header("Wave Data")]
	[SerializeField] private Vector3 _waveAmplitude = new Vector3(0, 15f, 0);
	[SerializeField] private float _periodDuration = 3f;
	[SerializeField] private float _startDelay = 0f;

	// Tween
	private Tween _positionTween;

	private void Start() {
		_defaultRectPosition = _rectTransform.localPosition;
		StartCoroutine(DelayTween(_startDelay));
	}

	private void OnDestroy() {
		if (_positionTween.isAlive) {
			_positionTween.Stop();
		}
	}

	private IEnumerator DelayTween(float delay) {
		yield return new WaitForSeconds(_startDelay);
		_positionTween = Tween.Custom(_defaultRectPosition + _waveAmplitude, _defaultRectPosition - _waveAmplitude, _periodDuration, newVal => _rectTransform.localPosition = newVal, Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
	}
}
