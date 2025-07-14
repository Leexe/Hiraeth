using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {
	[SerializeField] private float _maxHealth = 2f;

	[Header("Regeneration")]
	[SerializeField] private bool _toggleRegeneration = false;
	[SerializeField] private float _regenerationTime = 1f;

	[Header("Invincibility")]
	[SerializeField] private bool _enableInvincibilityFrames = false;
	[SerializeField] private float _invincibilityTime = 0f;

	private bool _canTakeDamage => _invincibilityTimer <= 0f;
	private float _invincibilityTimer = 0f;
	private float _health;
	private float _regenerationTimer = 0f;
	private bool _isDead = false;

	[HideInInspector] public UnityEvent OnDeath;
	[HideInInspector] public UnityEvent OnRegen;
	[HideInInspector] public UnityEvent<float, float> OnDamage; // Parameters: health, maxHealth

	public bool IsFullHealth => _maxHealth <= _health;
	public float GetHealth => _health;
	public float GetMaxHealth => _maxHealth;

	private void Start() {
		_health = _maxHealth;
		_regenerationTimer = _regenerationTime;

		// Event Listener
		GameManager.Instance.OnGameWin.AddListener(ToggleGodMode);
	}

	private void Update() {
		HandleRegeneration(Time.deltaTime);

		// Handle Invincibility
		if (!_canTakeDamage) {
			_invincibilityTimer -= Time.deltaTime;
		}
	}

	private void HandleRegeneration(float deltaTime) {
		if (_toggleRegeneration && !IsFullHealth) {
			_regenerationTimer -= deltaTime;
			if (_regenerationTimer <= 0f) {
				HealHealth(1f);
				_regenerationTimer = _regenerationTime;
				OnRegen?.Invoke();
			}
		}
	}

	private void HealHealth(float healing) {
		_health += healing;
	}

	private void ToggleGodMode() {
		_invincibilityTimer = float.MaxValue;
	}

	public void TakeDamage(float damage) {
		// Don't take damage if the player has died
		if (!_isDead && _canTakeDamage) {
			_health -= damage;

			// If the player is below a certain threshold of health, trigger death
			if (_health <= 0.1f) {
				_toggleRegeneration = false;
				OnDamage?.Invoke(_health, _maxHealth);
				OnDeath?.Invoke();
				_isDead = true;
			}
			// Else, damage them
			else {
				OnDamage?.Invoke(_health, _maxHealth);
				// Give the player invincibility frames if it is enabled
				if (_enableInvincibilityFrames)
					_invincibilityTimer = _invincibilityTime;
			}
		}
	}
}
