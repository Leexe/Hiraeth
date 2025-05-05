using Unity.Cinemachine;
using UnityEngine;
using PrimeTween;

public class CameraManager : Singleton<CameraManager> {
    [Header("References")]
    [SerializeField] private CinemachineCamera _firstPersonCamera;
    [SerializeField] private MyCharacterController _characterController;

    [Header("FPS Camera Settings")]
    [SerializeField] private float defaultFov = 60f;

    [Header("Movement")]
    [Tooltip("How intense the fov change is")]
    [SerializeField] private float _dashFovTargetFovPlus;
    [Tooltip("How long to zoom player camera out")]
    [SerializeField] private float _dashFovZoomOutDuration;

    private void Start() {
        _characterController.OnAirDash.AddListener(FovZoomOutEffect);
        _characterController.OnGroundDash.AddListener(FovZoomOutEffect);

        // Set default values for FPS camera
        _firstPersonCamera.Lens.FieldOfView = defaultFov;
    }

    private void OnDisable() {
        _characterController.OnAirDash.RemoveListener(FovZoomOutEffect);
        _characterController.OnGroundDash.RemoveListener(FovZoomOutEffect);
    }

    private void FovZoomOutEffect() {
        Tween.Custom(_firstPersonCamera.Lens.FieldOfView, _firstPersonCamera.Lens.FieldOfView + _dashFovTargetFovPlus, _dashFovZoomOutDuration, onValueChange: newVal => _firstPersonCamera.Lens.FieldOfView = newVal, Ease.Default, 2, CycleMode.Yoyo);
    }
}
