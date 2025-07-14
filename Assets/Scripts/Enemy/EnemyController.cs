using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour {
	[Header("References")]
	[SerializeField] private HealthManager _healthManager;
	[SerializeField] private Collider _physicsCollider;
	[SerializeField] private Collider _hurtboxCollider;
	[SerializeField] private GameObject _bulletPrefab;

	[Header("Enemy Data")]
	[SerializeField] private int _staminaGivenOnDeath = 1;
	[SerializeField] private float _damage = 1f;
	[SerializeField] private float _bulletVelocity = 100f;
	[SerializeField] private float _fireRate = 2f;

	[Header("Misc")]
	[SerializeField] private LayerMask _playerLayerMask;

	// Events
	[HideInInspector] public UnityEvent OnAttack;

	private float _attackTimer = 0f;
	private bool _playerInRange = false;
	private Transform _playerTransform;
	private bool _canAttack = true;

	private void Start() {
		_healthManager.OnDeath.AddListener(OnDeath);
		GameManager.Instance.OnGameWin.AddListener(DisableShooting);
		GameManager.Instance.OnGameLose.AddListener(DisableShooting);
	}

	private void OnDestroy() {
		_healthManager.OnDeath.RemoveListener(OnDeath);
		GameManager.Instance?.OnGameWin.RemoveListener(DisableShooting);
		GameManager.Instance?.OnGameLose.RemoveListener(DisableShooting);
	}

	private void Update() {
		HandleAttack(Time.deltaTime);
	}

	private void HandleAttack(float deltaTime) {
		if (_canAttack && _attackTimer <= 0 && _playerInRange) {
			OnAttack?.Invoke();
			Vector3 directionToPlayerNormalized = (_playerTransform.position - transform.position + new Vector3(0f, 1f, 0f)).normalized;

			// Instantiate current bullet
			GameObject currBullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
			// Rotate bullet to the correct direction
			currBullet.transform.up = directionToPlayerNormalized;

			// Add forces to bullet
			currBullet.GetComponent<Rigidbody>().AddForce(directionToPlayerNormalized.normalized * _bulletVelocity, ForceMode.Impulse);

			// Populate bullet data
			currBullet.GetComponent<BulletManager>().InitiateBullet(_damage);

			_attackTimer = _fireRate;
		}
		_attackTimer -= deltaTime;
	}


	private void OnTriggerEnter(Collider other) {
		if (((1 << other.gameObject.layer) & _playerLayerMask) > 1) {
			_playerInRange = true;
			_playerTransform = other.transform;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (((1 << other.gameObject.layer) & _playerLayerMask) > 1) {
			_playerInRange = false;
		}
	}

	private void DisableObjectOnDeath() {
		gameObject.SetActive(false);
	}

	private void DisableShooting() {
		_canAttack = false;
	}

	private void OnDeath() {
		_hurtboxCollider.enabled = false;
		_physicsCollider.enabled = false;
		_canAttack = false;
		GameManager.Instance.OnEnemyDeath?.Invoke(_staminaGivenOnDeath);
	}
}