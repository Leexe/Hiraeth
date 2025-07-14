using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MyPlayer : MonoBehaviour {
	[Header("References")]
	[SerializeField] private MyCharacterController _characterController;
	[SerializeField] private ShootingSystem _shootingSystem;
	[SerializeField] private InputActionAsset _originalInputAction;
	private Transform _cameraTransform;
	private PlayerInput _playerInput;

	[Header("Misc.")]
	[SerializeField] private float _doubleCrouchBuffer = 0.5f;

	private Vector3 _horizontalMovementInput;
	private bool _dashPerformed = false;
	private bool _jumpPerformed = false;
	private bool _reloadPerformed = false;
	private bool _shootingDown = false;
	private bool _crouchDown = false;
	private bool _doubleCrouched = false;
	private float _crouchPressedTimer = 0f;

	private InputAction _resetAction;
	private InputAction _moveAction;
	private InputAction _jumpAction;
	private InputAction _dashAction;
	private InputAction _crouchAction;
	private InputAction _shootAction;
	private InputAction _reloadAction;
	private InputAction _escapeAction;
	private InputAction _anyAction;

	private string _movmentActionMapName = "Movement";
	private string _escapeActionMapName = "EscapeMenu";

	private void Awake() {
		_playerInput = GetComponent<PlayerInput>();
		SetupInputActions();
	}

	private void Start() {
		GameManager.Instance.OnGameLose.AddListener(DisableMovementInput);
		GameManager.Instance.OnGameWin.AddListener(DisableMovementInput);
		GameManager.Instance.OnPause.AddListener(DisableMovementInput);
		GameManager.Instance.OnResume.AddListener(CopyOverKeybinds);
		GameManager.Instance.OnResume.AddListener(EnableMovementInput);
		GameManager.Instance.OnReset.AddListener(DisableEscapeMenuInput);
		LevelManger.Instance.OnLevelStartTransitionFinish.AddListener(EnableEscapeMenuInput);
		LevelManger.Instance.OnLevelStartTransitionFinish.AddListener(EnableMovementInput);

		_cameraTransform = CameraManager.Instance.MainCameraGameObject.transform;
	}

	private void OnDisable() {
		GameManager.Instance?.OnGameLose.RemoveListener(DisableMovementInput);
		GameManager.Instance?.OnGameWin.RemoveListener(DisableMovementInput);
		GameManager.Instance?.OnPause.RemoveListener(DisableMovementInput);
		GameManager.Instance?.OnResume.RemoveListener(CopyOverKeybinds);
		GameManager.Instance?.OnResume.RemoveListener(EnableMovementInput);
		GameManager.Instance?.OnReset.RemoveListener(DisableEscapeMenuInput);
		LevelManger.Instance?.OnLevelStartTransitionFinish.RemoveListener(EnableEscapeMenuInput);
		LevelManger.Instance?.OnLevelStartTransitionFinish.RemoveListener(EnableMovementInput);
	}

	private void SetupInputActions() {
		_moveAction = _playerInput.actions["Move"];
		_jumpAction = _playerInput.actions["Jump"];
		_dashAction = _playerInput.actions["Dash"];
		_resetAction = _playerInput.actions["Reset"];
		_crouchAction = _playerInput.actions["Crouch"];
		_shootAction = _playerInput.actions["Shoot"];
		_reloadAction = _playerInput.actions["Reload"];
		_escapeAction = _playerInput.actions["Escape"];
		_anyAction = _playerInput.actions["Any"];

		DisableMovementInput();
		DisableEscapeMenuInput();
	}

	private void Update() {
		UpdateInputs();
		HandleCharacterInputs();
		HandleGunInputs();
		HandleDoubleCrouch(Time.deltaTime);
	}

	private void UpdateInputs() {
		_horizontalMovementInput = _moveAction.ReadValue<Vector3>();
		_dashPerformed = _dashAction.WasPressedThisFrame();
		_jumpPerformed = _jumpAction.WasPressedThisFrame();
		_reloadPerformed = _reloadAction.WasPressedThisFrame();

		if (_escapeAction.WasPressedThisFrame()) {
			GameManager.Instance.OnTogglePauseMenu.Invoke();
		}

		if (_resetAction.WasPressedThisFrame()) {
			GameManager.Instance.OnReset?.Invoke();
		}

		if (_anyAction.WasPressedThisFrame()) {
			GameManager.Instance.OnTimerStart?.Invoke();
		}

		IsHoldingDownButton(_shootAction, ref _shootingDown);
		IsHoldingDownButton(_crouchAction, ref _crouchDown);
	}

	private void IsHoldingDownButton(InputAction inputAction, ref bool keyIsHeld) {
		if (inputAction.WasPressedThisFrame()) {
			keyIsHeld = true;
		}

		if (inputAction.WasReleasedThisFrame()) {
			keyIsHeld = false;
		}
	}

	private void HandleDoubleCrouch(float deltaTime) {
		if (_crouchAction.WasPressedThisFrame()) {
			if (_crouchPressedTimer > 0f) {
				_doubleCrouched = true;
			}
			_crouchPressedTimer = _doubleCrouchBuffer;
		}
		_crouchPressedTimer -= deltaTime;
	}

	/** Inputs **/

	private void HandleCharacterInputs() {
		CharacterControllerPlayerInput.Inputs playerInputs = new CharacterControllerPlayerInput.Inputs();

		playerInputs.MovementInput = _horizontalMovementInput.normalized;
		playerInputs.CameraRotation = _cameraTransform.rotation;
		playerInputs.CrouchDown = _crouchDown;
		playerInputs.DoubleCrouched = _doubleCrouched;
		_doubleCrouched = false;
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

	private void HandleGunInputs() {
		_shootingSystem.ShootingDown = _shootingDown;
		if (_reloadPerformed) {
			_shootingSystem.ReloadPerformed = _reloadPerformed;
			_reloadPerformed = false;
		}
	}

	private void CopyOverridenBindings(InputActionMap copyActionMap, InputActionMap originalActionMap) {
		for (int i = 0; i < copyActionMap.actions.Count; i++) {
			var copyAction = copyActionMap.actions[i];
			var originalAction = originalActionMap.actions[i];
			for (int j = 0; j < copyAction.bindings.Count; j++) {
				var originalBinding = originalAction.bindings[j];
				if (originalBinding.overridePath != null
					&& originalBinding.overridePath != string.Empty) {
					copyAction.ChangeBinding(j).WithPath(originalBinding.overridePath);
				}
			}
		}
	}

	private void CopyOverKeybinds() {
		CopyOverridenBindings(_playerInput.actions.FindActionMap(_movmentActionMapName), _originalInputAction.FindActionMap(_movmentActionMapName));
	}

	private void EnableMovementInput() {
		_playerInput.actions.FindActionMap(_movmentActionMapName).Enable();
	}

	private void DisableMovementInput() {
		_playerInput.actions.FindActionMap(_movmentActionMapName).Disable();
	}

	private void EnableEscapeMenuInput() {
		_playerInput.actions.FindActionMap(_escapeActionMapName).Enable();
	}

	private void DisableEscapeMenuInput() {
		_playerInput.actions.FindActionMap(_escapeActionMapName).Disable();
	}
}