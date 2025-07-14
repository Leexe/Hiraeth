using UnityEngine;
using UnityEngine.Events;

public class ShootingSystem : MonoBehaviour {
	[Header("References")]
	[SerializeField] private Transform _characterTransform;
	[SerializeField] private Transform _gunShootTransform;
	[SerializeField] private GameObject _bulletPrefab;
	private Camera _fpsCamera;

	[Header("Gun Data")]
	[SerializeField] private float _damage = 1f;
	[SerializeField] private int _maxAmmo = 6;
	[SerializeField] private float _bulletVelocity = 100f;
	[SerializeField] private float _fireBuffer = 0.1f;
	[SerializeField] private float _reloadBuffer = 0.1f;
	[SerializeField] private AnimationCurve _bulletSpreadCurve;
	[SerializeField] private float _bulletSpreadAdd = 0.8f;

	[Header("Hit Data")]
	[SerializeField] private float _bulletDistance = 25f;

	[Header("Misc")]
	[SerializeField] private LayerMask _enemyLayerMask;

	private HandAnimations _handAnimations;
	private bool _canShoot => _shootingCooldownTimer <= 0f && _reloadTimer <= 0f && _ammo > 0;
	private bool _canReload => _shootingCooldownTimer <= 0f && _reloadTimer <= 0f && _ammo < _maxAmmo;
	// Gun Info
	private float _reloadTime = 2.5f;
	private float _fireRate = 0.5f;
	private float _fireBufferTimer = 0f;
	private float _reloadBufferTimer = 0f;
	private float _shootingCooldownTimer = 0f;
	private float _reloadTimer = 0f;
	private float _bulletSpreadBuildUp = 0f;
	private float _maxBulletSpreadBuildUp = 1f;
	private int _ammo = 0;

	public bool ShootingDown { set; private get; }
	public bool ReloadPerformed { set; private get; }
	public bool IsReloading => _reloadTimer > 0f;
	public bool IsShooting => _shootingCooldownTimer > 0f;
	public float GetMaxAmmo => _maxAmmo;
	public float GetAmmo => _ammo;

	[HideInInspector] public UnityEvent OnGunShoot;
	[HideInInspector] public UnityEvent OnGunReload;

	private void Start() {
		_ammo = _maxAmmo;
		_maxBulletSpreadBuildUp = _bulletSpreadCurve.keys[_bulletSpreadCurve.length - 1].time;
		_handAnimations = GameManager.Instance.HandAnimationsRef;
		_reloadTime = _handAnimations.GetReloadDuration;
		_fireRate = _handAnimations.GetShootDuration;
		_fpsCamera = CameraManager.Instance.MainCameraGameObject.GetComponent<Camera>();
	}

	private void Update() {
		HandleShootingDown(Time.deltaTime);
		HandleReloadPerformed(Time.deltaTime);
		DealWithFireRate(Time.deltaTime);
		DealWithReloadTime(Time.deltaTime);
		DealWithSpread(Time.deltaTime);
		if ((_canReload && _reloadBufferTimer > 0f) || (_ammo == 0 && !IsShooting)) {
			Reload();
		}
		else if (_canShoot && _fireBufferTimer > 0f) {
			ShootGun();
		}
	}

	private void HandleShootingDown(float deltaTime) {
		if (ShootingDown) {
			_fireBufferTimer = _fireBuffer;
		}
		else if (_fireBufferTimer > 0f) {
			_fireBufferTimer -= deltaTime;
		}
	}

	private void HandleReloadPerformed(float deltaTime) {
		if (ReloadPerformed) {
			_reloadBufferTimer = _reloadBuffer;
			ReloadPerformed = false;
		}
		else if (_reloadBufferTimer > 0f) {
			_reloadBufferTimer -= deltaTime;
		}
	}

	private void DealWithFireRate(float deltaTime) {
		if (_shootingCooldownTimer > 0f) {
			_shootingCooldownTimer -= deltaTime;
		}
	}

	private void DealWithReloadTime(float deltaTime) {
		if (_reloadTimer > 0f) {
			_reloadTimer -= deltaTime;
		}
	}

	private void DealWithSpread(float deltaTime) {
		if (_bulletSpreadBuildUp > _maxBulletSpreadBuildUp) {
			_bulletSpreadBuildUp = _maxBulletSpreadBuildUp;
		}
		else if (_bulletSpreadBuildUp > 0f) {
			_bulletSpreadBuildUp -= deltaTime;
		}
	}

	public void ShootGun() {
		_ammo--;
		_shootingCooldownTimer = _fireRate;

		// Shoot out a ray in the middle of the fps camera
		Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit bulletHit;
		bool _isEnemyHit = Physics.Raycast(ray, out bulletHit);

		// Find the first world space point that the ray intersects
		Vector3 targetPoint;
		if (_isEnemyHit) {
			targetPoint = bulletHit.point;
		}
		else {
			targetPoint = ray.GetPoint(_bulletDistance);
		}

		// Calculate the direction velocity from the gun position to the target point
		Vector3 directionWithoutSpread = targetPoint - _gunShootTransform.position;

		// Apply spread
		float spread = _bulletSpreadCurve.Evaluate(_bulletSpreadBuildUp);
		float spreadX = Random.Range(-spread, spread);
		float spreadY = Random.Range(-spread, spread);
		Vector3 directionWithSpread = directionWithoutSpread + new Vector3(spreadX, spreadY, 0);

		// Instantiate current bullet
		GameObject currBullet = Instantiate(_bulletPrefab, _gunShootTransform.position, Quaternion.identity);
		// Rotate bullet to the correct direction
		currBullet.transform.up = directionWithSpread.normalized;

		// Add forces to bullet
		currBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * _bulletVelocity, ForceMode.Impulse);

		// Populate bullet data
		currBullet.GetComponent<BulletManager>().InitiateBullet(_damage);

		OnGunShoot?.Invoke();
		_bulletSpreadBuildUp += _bulletSpreadAdd;
	}

	public void Reload() {
		_ammo = _maxAmmo;
		_reloadTimer = _reloadTime;
		OnGunReload?.Invoke();
	}

	public float GetBulletSpread() {
		return _bulletSpreadBuildUp;
	}
}
