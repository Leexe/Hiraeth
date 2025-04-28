using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayer : MonoBehaviour {
    [SerializeField] private MyCharacterController _characterController;
    [SerializeField] private Transform _cameraTransform;

    private Actions _inputs;
    private Vector3 _horizontalMovementInput;
    private bool _dashPerformed = false;
    private bool _jumpPerformed = false;
    private bool _crouchDown = false;

    private void Awake() {
        _inputs = new Actions();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() {
        _inputs.Movement.Enable();
        _inputs.Movement.Crouch.performed += OnCrouchInputPerformed;
        _inputs.Movement.Crouch.canceled += OnCrouchInputCanceled;
        _inputs.Movement.Jump.performed += OnJumpInputPerformed;
        _inputs.Movement.Dash.performed += OnDashInputPerformed;
    }

    private void OnDisable() {
        _inputs.Movement.Disable();
        _inputs.Movement.Crouch.performed -= OnCrouchInputPerformed;
        _inputs.Movement.Crouch.canceled -= OnCrouchInputCanceled;
        _inputs.Movement.Jump.performed += OnJumpInputPerformed;
        _inputs.Movement.Dash.performed += OnDashInputPerformed;
    }

    private void Update() {
        _horizontalMovementInput = _inputs.Movement.Horizontal.ReadValue<Vector3>();
        HandleCharacterInputs();
    }

    /** Inputs **/

    private void OnCrouchInputPerformed(InputAction.CallbackContext context) {
        _crouchDown = true;
    }

    private void OnCrouchInputCanceled(InputAction.CallbackContext context) {
        _crouchDown = false;
    }

    private void OnJumpInputPerformed(InputAction.CallbackContext context) {
        _jumpPerformed = true;
    }

    private void OnDashInputPerformed(InputAction.CallbackContext context) {
        _dashPerformed = true;
    }

    private void HandleCharacterInputs() {
        CharacterControllerPlayerInput.Inputs playerInputs = new CharacterControllerPlayerInput.Inputs();

        playerInputs.MovementInput = _horizontalMovementInput.normalized;
        playerInputs.CameraRotation = _cameraTransform.rotation;
        playerInputs.CrouchDown = _crouchDown;
        if (_jumpPerformed) {
            playerInputs.JumpRequested = true;
            _jumpPerformed = false;
        }
        if (_dashPerformed) {
            playerInputs.DashRequested = true;
            _dashPerformed = false;
        }

        _characterController.FeedPlayerInput(ref playerInputs);
    }
}