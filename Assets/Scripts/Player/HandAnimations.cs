using UnityEngine;

public class HandAnimations : MonoBehaviour {
	[Header("Reference")]
	[SerializeField] private Animator _animator;
	[SerializeField] private AnimationClip _reloadAnimationClip;
	[SerializeField] private AnimationClip _shootAnimationClip;
	private MyCharacterController _myCharacterController;
	private ShootingSystem _shootingSystem;

	// Animator Names
	private int _reloadName;
	private int _shootName;
	private int _speedName;
	private int _inAirName;
	private int _slidingName;
	private int _groundDashName;
	private int _airDashName;

	// Private Variables
	private float _reloadDuration;
	private float _shootDuration;
	private bool _sliding = false;
	private bool _groundDash = false;
	private bool _airDash = false;

	// Getters and Setters
	public float GetReloadDuration => _reloadAnimationClip.length;
	public float GetShootDuration => _shootAnimationClip.length;

	private void Start() {
		_myCharacterController = GameManager.Instance.MyCharacterControllerRef;
		_shootingSystem = GameManager.Instance.ShootingSystemRef;

		_reloadName = Animator.StringToHash("Reload");
		_shootName = Animator.StringToHash("Shoot");
		_speedName = Animator.StringToHash("HorizontalSpeed");
		_inAirName = Animator.StringToHash("InAir");
		_slidingName = Animator.StringToHash("GunSliding");
		_groundDashName = Animator.StringToHash("GroundDash");
		_airDashName = Animator.StringToHash("AirDash");

		_shootingSystem.OnGunReload.AddListener(PlayReloadAnimation);
		_shootingSystem.OnGunShoot.AddListener(PlayShootAnimation);
		_myCharacterController.OnAirDash.AddListener(ToggleAirDashBool);
		_myCharacterController.OnAirDashEnd.AddListener(ToggleAirDashBool);
		_myCharacterController.OnGroundDash.AddListener(ToggleGroundBool);
		_myCharacterController.OnGroundDashEnd.AddListener(ToggleGroundBool);
		_myCharacterController.OnSlideStart.AddListener(ToggleSlidingBool);
		_myCharacterController.OnSlideEnd.AddListener(ToggleSlidingBool);
	}

	private void Update() {
		_animator.SetBool(_inAirName, !_myCharacterController.IsGrounded);
		_animator.SetFloat(_speedName, _myCharacterController.CurrentHorVelocity.magnitude);
	}

	private void ToggleAirDashBool() {
		_airDash = !_airDash;
		_animator.SetBool(_airDashName, _airDash);
	}

	private void ToggleGroundBool() {
		_groundDash = !_groundDash;
		_animator.SetBool(_groundDashName, _groundDash);
	}

	private void ToggleSlidingBool() {
		_sliding = !_sliding;
		_animator.SetBool(_slidingName, _sliding);
	}

	private void PlayReloadAnimation() {
		_animator.SetTrigger(_reloadName);
	}

	private void PlayShootAnimation() {
		_animator.SetTrigger(_shootName);
	}

}
