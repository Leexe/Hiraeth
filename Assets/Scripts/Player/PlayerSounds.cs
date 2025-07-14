using UnityEngine;
using FMOD.Studio;

public class PlayerSounds : MonoBehaviour {
	[Header("References")]
	[SerializeField] private MyCharacterController _characterController;
	[SerializeField] private ShootingSystem _shootingSystem;
	[SerializeField] private StaminaSystem _staminaSystem;
	[SerializeField] private GameObject _characterGameObject;
	[SerializeField] private HealthManager _healthManger;

	private EventInstance _slideStartInstance;
	private EventInstance _wallRunInstance;

	private void Start() {
		// Create Instances
		_slideStartInstance = AudioManager.Instance.CreateInstanceAndAttach(FMODEvents.Instance.Slide_sfx, _characterGameObject);
		_wallRunInstance = AudioManager.Instance.CreateInstanceAndAttach(FMODEvents.Instance.WallRun_sfx, _characterGameObject);
	}

	private void OnEnable() {
		// Movement Events
		_characterController.OnGroundJump.AddListener(PlayGroundJumpSound);
		_characterController.OnAirJump.AddListener(PlayAirJumpSound);
		_characterController.OnGroundDash.AddListener(PlayGroundDashSound);
		_characterController.OnAirDash.AddListener(PlayAirDashSound);
		_characterController.OnLanding.AddListener(PlayLandSound);
		_characterController.OnSlideStart.AddListener(PlaySlideStartSounds);
		_characterController.OnSlideEnd.AddListener(HandleSlideEndSounds);
		_characterController.OnWallJump.AddListener(PlayWallJumpSound);
		_characterController.OnWallRunStart.AddListener(PlayWallRunSounds);
		_characterController.OnWallRunEnd.AddListener(StopWallRunSounds);
		_characterController.OnDownwardsDash.AddListener(PlayDownwardsDashSound);

		// Character Events
		_healthManger.OnDamage.AddListener(PlayPlayerHurtSound);

		// Stamina Events
		// _staminaSystem.OnStaminaRecharged.AddListener(PlayStaminaRegenSound); // Currently sounds annoying

		// Gun Events
		_shootingSystem.OnGunShoot.AddListener(PlayGunShootSound);
		_shootingSystem.OnGunReload.AddListener(PlayGunReloadSound);
	}

	private void OnDisable() {
		// Movement Events
		_characterController.OnGroundJump.RemoveListener(PlayGroundJumpSound);
		_characterController.OnAirJump.RemoveListener(PlayAirJumpSound);
		_characterController.OnGroundDash.RemoveListener(PlayGroundDashSound);
		_characterController.OnAirDash.RemoveListener(PlayAirDashSound);
		_characterController.OnLanding.RemoveListener(PlayLandSound);
		_characterController.OnSlideStart.RemoveListener(PlaySlideStartSounds);
		_characterController.OnSlideEnd.RemoveListener(HandleSlideEndSounds);
		_characterController.OnWallJump.RemoveListener(PlayWallJumpSound);
		_characterController.OnWallRunStart.RemoveListener(PlayWallRunSounds);
		_characterController.OnWallRunEnd.RemoveListener(StopWallRunSounds);
		_characterController.OnDownwardsDash.RemoveListener(PlayDownwardsDashSound);

		// Character Events
		_healthManger.OnDamage.RemoveListener(PlayPlayerHurtSound);

		// Gun Events
		_shootingSystem.OnGunShoot.RemoveListener(PlayGunShootSound);
		_shootingSystem.OnGunReload.RemoveListener(PlayGunReloadSound);
	}

	private void PlayGroundJumpSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Jump_sfx, _characterGameObject);
	}

	private void PlayAirJumpSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.AirJump_sfx, _characterGameObject);
	}

	private void PlayGroundDashSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.GroundedDash_sfx, _characterGameObject);
	}

	private void PlayAirDashSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.AirDash_sfx, _characterGameObject);
	}

	private void PlayLandSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Land_sfx, _characterGameObject);
	}

	private void PlayWallJumpSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.WallJump_sfx, _characterGameObject);
	}

	private void PlayGunShootSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.GunShoot_sfx, _characterGameObject);
	}

	private void PlayGunReloadSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Reload_sfx, _characterGameObject);
	}

	private void PlayPlayerHurtSound(float _health, float _maxHealth) {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerHurt_sfx, Vector3.zero);
	}

	private void PlayDownwardsDashSound() {
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.DownwardsDash_sfx, Vector3.zero);
	}

	private void PlaySlideStartSounds() {
		_slideStartInstance.start();
	}

	private void HandleSlideEndSounds() {
		_slideStartInstance.stop(STOP_MODE.ALLOWFADEOUT);
	}

	private void PlayWallRunSounds(bool isOnRightWall) {
		_wallRunInstance.start();
	}

	private void StopWallRunSounds() {
		_wallRunInstance.stop(STOP_MODE.ALLOWFADEOUT);
	}
}
