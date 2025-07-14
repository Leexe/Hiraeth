using PrimeTween;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Deprecated, I can't figure out how to modify the volume

public class VolumeManger : MonoBehaviour {
	[Header("References")]
	[SerializeField] private Volume _volumeProfile;
	private Camera _mainCamera;
	private Vignette _vignette;

	[Header("Vignette Default Values")]
	[SerializeField] private Color _vignetteDefaultColor = Color.white;
	[SerializeField] private float _vignetteDefaultIntensity = 0.2f;
	[SerializeField] private float _vignetteDefaultSmoothness = 0.2f;

	[Header("Vignette Transition Values")]
	[SerializeField] private float _vignetteTransitionTime = 0.4f;
	[SerializeField] private Color _vignetteTransitionColor = Color.red;
	[SerializeField] private float _vignetteTransitionIntensity = 0.5f;
	[SerializeField] private float _vignetteTransitionSmoothness = 0.4f;

	private void Start() {
		// Get References
		_volumeProfile.sharedProfile.TryGet<Vignette>(out _vignette);
		_mainCamera = CameraManager.Instance.MainCameraGameObject.GetComponent<Camera>();

		// Set Default Values
		_vignette.color.value = _vignetteDefaultColor;
		_vignette.intensity.value = _vignetteDefaultIntensity;
		_vignette.smoothness.value = _vignetteDefaultSmoothness;

		// Add Listeners
		// GameManager.Instance.OnPause.AddListener(PauseVignetteStart);
		// GameManager.Instance.OnResume.AddListener(PauseVignetteEnd);
	}

	private void PauseVignetteStart() {
		Tween.Custom(_vignette.color.value, _vignetteTransitionColor, _vignetteTransitionTime, newVal => _vignette.color.value = newVal, useUnscaledTime: true);
		Tween.Custom(_vignette.intensity.value, _vignetteTransitionIntensity, _vignetteTransitionTime, newVal => _vignette.intensity.value = newVal, useUnscaledTime: true);
		Tween.Custom(_vignette.smoothness.value, _vignetteTransitionSmoothness, _vignetteTransitionTime, newVal => _vignette.smoothness.value = newVal, useUnscaledTime: true);
		Tween.Custom(_vignette.smoothness.value, _vignetteTransitionSmoothness, _vignetteTransitionTime, newVal => CameraExtensions.UpdateVolumeStack(_mainCamera), useUnscaledTime: true);
	}

	private void PauseVignetteEnd() {
		Tween.Custom(_vignette.color.value, _vignetteDefaultColor, _vignetteTransitionTime, newVal => _vignette.color.value = newVal, useUnscaledTime: true);
		Tween.Custom(_vignette.intensity.value, _vignetteDefaultIntensity, _vignetteTransitionTime, newVal => _vignette.intensity.value = newVal, useUnscaledTime: true);
		Tween.Custom(_vignette.smoothness.value, _vignetteDefaultSmoothness, _vignetteTransitionTime, newVal => _vignette.smoothness.value = newVal, useUnscaledTime: true);
		Tween.Custom(_vignette.smoothness.value, _vignetteTransitionSmoothness, _vignetteTransitionTime, newVal => CameraExtensions.UpdateVolumeStack(_mainCamera), useUnscaledTime: true);
	}
}
