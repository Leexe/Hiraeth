using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour {
	[Header("References")]
	[SerializeField] private RectTransform _crosshair;
	private ShootingSystem _shootingSystem;

	[Header("Data")]
	[SerializeField] private Vector2 defaultSize = new Vector2(25, 25);
	[SerializeField] private AnimationCurve _recoilCurve;

	private void Start() {
		_shootingSystem = GameManager.Instance.ShootingSystemRef;
		_crosshair.sizeDelta = defaultSize;
	}

	private void Update() {
		float bulletSpread = _shootingSystem.GetBulletSpread();
		float newSize = _recoilCurve.Evaluate(bulletSpread);
		_crosshair.sizeDelta = new Vector2(newSize, newSize);
	}
}
