using Unity.Cinemachine;
using UnityEngine;
using PrimeTween;
using UnityEngine.Rendering.Universal;

public class CameraManager : Singleton<CameraManager> {
	[Header("References")]
	public GameObject MainCameraGameObject;
	public GameObject FPSCameraGameObject;
	private MyCharacterController _characterController;
	private CinemachineInputAxisController _cinemachineInputAxisController;
	private CinemachineBasicMultiChannelPerlin _cinemachineMultiChannelPerlin;
	private CinemachineCamera _cinemachineCamera;
	private Camera _camera;

	[Header("FPS Camera Settings")]
	[SerializeField] private float _defaultFov = 60f;
	[SerializeField] private float _defaultCameraSensitivity = 1.125f;
	public float _cameraSensitivity { get; private set; }

	[Header("Movement")]
	[Tooltip("How intense the fov change is")]
	[SerializeField] private float _dashFovTargetFovPlus;
	[Tooltip("How long to zoom player camera out")]
	[SerializeField] private float _dashFovZoomOutDuration;

	[Header("Hurt")]
	[Tooltip("How intense the hurt camera shake is")]
	[SerializeField] private float _hurtCameraShakeAmplitude = 3f;
	[Tooltip("How often the hurt camera shake is")]
	[SerializeField] private float _hurtCameraShakeFrequency = 1f;
	[Tooltip("How long the hurt camera shake takes")]
	[SerializeField] private float _hurtCameraShakeDuration = 0.5f;

	[Header("Wall Running")]
	[Tooltip("How deep the angle is for wall running")]
	[SerializeField] private float _wallRunningCameraTilt = 10;
	[Tooltip("How fast the camera tilts when wall running")]
	[SerializeField] private float _wallRunningCameraTiltDuration = 0.25f;

	// Tween
	private Tween CameraShakeAmplitudeTween;
	private Tween CameraShakeFrequencyTween;
	private Tween FOVTween;
	private Tween DutchTween;

	override protected void Awake() {
		base.Awake();

		// Get References
		_cinemachineCamera = FPSCameraGameObject.GetComponent<CinemachineCamera>();
		_cinemachineInputAxisController = FPSCameraGameObject.GetComponent<CinemachineInputAxisController>();
		_cinemachineMultiChannelPerlin = FPSCameraGameObject.GetComponent<CinemachineBasicMultiChannelPerlin>();
		_camera = MainCameraGameObject.GetComponent<Camera>();

		// Get values from file
		_cameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
	}

	private void Start() {
		_characterController = GameManager.Instance.MyCharacterControllerRef;

		// Events Listeners
		_characterController.OnAirDash.AddListener(FovZoomOutEffect);
		_characterController.OnGroundDash.AddListener(FovZoomOutEffect);
		_characterController.OnWallRunStart.AddListener(CameraTiltStartEffect);
		_characterController.OnWallRunEnd.AddListener(CameraTiltEndEffect);
		GameManager.Instance.OnGameLose.AddListener(DisableMouseMovement);
		GameManager.Instance.OnGameWin.AddListener(DisableMouseMovement);
		HealthManager healthManger = GameManager.Instance._characterGameObject.GetComponent<HealthManager>();
		healthManger.OnDamage.AddListener(DamagedToHurtListener);
		healthManger.OnDeath.AddListener(HurtCameraShake);

		// Set default values for FPS camera
		_cinemachineCamera.Lens.FieldOfView = _defaultFov;
		ChangeSensitivity(_cameraSensitivity);
	}

	private void OnDestroy() {
		// Unsubscribe from Events
		_characterController?.OnAirDash.RemoveListener(FovZoomOutEffect);
		_characterController?.OnGroundDash.RemoveListener(FovZoomOutEffect);
		_characterController?.OnWallRunStart.RemoveListener(CameraTiltStartEffect);
		_characterController?.OnWallRunEnd.RemoveListener(CameraTiltEndEffect);
		GameManager.Instance?.OnGameLose.RemoveListener(DisableMouseMovement);
		GameManager.Instance?.OnGameWin.RemoveListener(DisableMouseMovement);

		// Clean Up Tweens
		if (FOVTween.isAlive) FOVTween.Complete();
		if (DutchTween.isAlive) DutchTween.Complete();
		if (CameraShakeAmplitudeTween.isAlive) CameraShakeAmplitudeTween.Complete();
		if (CameraShakeFrequencyTween.isAlive) CameraShakeFrequencyTween.Complete();
	}

	public void ChangeSensitivity(float newSens) {
		foreach (var c in _cinemachineInputAxisController.Controllers) {
			if (c.Name == "Look X (Pan)") {
				c.Input.Gain = _defaultCameraSensitivity * newSens;
				_cameraSensitivity = newSens;
			}
			else if (c.Name == "Look Y (Tilt)") {
				c.Input.Gain = -_defaultCameraSensitivity * newSens;
				_cameraSensitivity = newSens;
			}
		}
	}

	private void FovZoomOutEffect() {
		FOVTween = Tween.Custom(_cinemachineCamera.Lens.FieldOfView, _cinemachineCamera.Lens.FieldOfView + _dashFovTargetFovPlus, _dashFovZoomOutDuration, onValueChange: newVal => _cinemachineCamera.Lens.FieldOfView = newVal, Ease.Default, 2, CycleMode.Yoyo);
	}

	private void CameraTiltStartEffect(bool isCloseToRightWall) {
		DutchTween = Tween.Custom(_cinemachineCamera.Lens.Dutch, isCloseToRightWall ? _wallRunningCameraTilt : -_wallRunningCameraTilt, _wallRunningCameraTiltDuration, onValueChange: newVal => _cinemachineCamera.Lens.Dutch = newVal, Ease.InOutSine);
	}

	private void CameraTiltEndEffect() {
		DutchTween = Tween.Custom(_cinemachineCamera.Lens.Dutch, 0, _wallRunningCameraTiltDuration, onValueChange: newVal => _cinemachineCamera.Lens.Dutch = newVal, Ease.InOutSine);
	}

	private void DamagedToHurtListener(float _health, float _maxHealth) {
		HurtCameraShake();
	}

	private void HurtCameraShake() {
		CameraShakeAmplitudeTween = Tween.Custom(_hurtCameraShakeAmplitude, 0f, _hurtCameraShakeDuration, newVal => _cinemachineMultiChannelPerlin.AmplitudeGain = newVal);
		CameraShakeFrequencyTween = Tween.Custom(_hurtCameraShakeFrequency, 0f, _hurtCameraShakeDuration, newVal => _cinemachineMultiChannelPerlin.FrequencyGain = newVal);
	}

	private void DisableMouseMovement() {
		_cinemachineInputAxisController.enabled = false;
	}
}
