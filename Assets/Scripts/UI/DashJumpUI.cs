using System.Collections.Generic;
using UnityEngine;

public class DashJumpUI : UI {
	[Header("References")]
	[SerializeField] private List<GameObject> _airDashIcons;
	[SerializeField] private List<GameObject> _jumpIcons;
	private MyCharacterController _myCharacterController;

	private void Start() {
		_myCharacterController = GameManager.Instance.MyCharacterControllerRef;

		_myCharacterController.OnAirDash.AddListener(UpdateDashUI);
		_myCharacterController.OnLanding.AddListener(UpdateDashUI);
		_myCharacterController.OnDashRefresh.AddListener(UpdateDashUI);
		_myCharacterController.OnSlideStart.AddListener(UpdateDashUI);

		_myCharacterController.OnGroundJump.AddListener(UpdateJumpUI);
		_myCharacterController.OnAirJump.AddListener(UpdateJumpUI);
		_myCharacterController.OnWallJump.AddListener(UpdateJumpUI);
		_myCharacterController.OnWallRunStart.AddListener(UpdateJumpUI);
		_myCharacterController.OnLanding.AddListener(UpdateJumpUI);
		_myCharacterController.OnAirJumpsRefresh.AddListener(UpdateJumpUI);
		_myCharacterController.OnSlideStart.AddListener(UpdateJumpUI);
		GameManager.Instance.OnGameWin.AddListener(DisableUI);
	}

	private void UpdateDashUI() {
		int dashes = _myCharacterController.GetAirDashes;
		if (dashes == 0) {
			_airDashIcons[0].SetActive(true);
			_airDashIcons[1].SetActive(true);
		}
		else if (dashes == 1) {
			_airDashIcons[0].SetActive(true);
			_airDashIcons[1].SetActive(false);
		}
		else {
			_airDashIcons[0].SetActive(false);
			_airDashIcons[1].SetActive(false);
		}
	}

	private void UpdateJumpUI() {
		int jumps = _myCharacterController.GetJumps;
		if (jumps == 2) {
			_jumpIcons[0].SetActive(true);
			_jumpIcons[1].SetActive(true);
		}
		else if (jumps == 1) {
			_jumpIcons[0].SetActive(true);
			_jumpIcons[1].SetActive(false);
		}
		else {
			_jumpIcons[0].SetActive(false);
			_jumpIcons[1].SetActive(false);
		}
	}

	private void UpdateJumpUI(bool temp) {
		UpdateJumpUI();
	}
}
