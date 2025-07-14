using UnityEngine;

public class HurtOverlayAnimator : MonoBehaviour {
	[Header("References")]
	[SerializeField] private Animator _animator;
	private HealthManager _playerHealthManger;

	// Animation Names
	private int _onHurtHash;
	private int _onRegenHash;

	private void Start() {
		// Get References
		_playerHealthManger = GameManager.Instance._characterGameObject.GetComponent<HealthManager>();

		// Get Animation Names
		_onHurtHash = Animator.StringToHash("OnHurt");
		_onRegenHash = Animator.StringToHash("OnRegen");

		// Add Event Listeners
		_playerHealthManger.OnDamage.AddListener(TriggerHurtAnimation);
		_playerHealthManger.OnRegen.AddListener(TriggerRegenAnimation);
	}

	private void TriggerHurtAnimation(float health, float _maxHealth) {
		_animator.SetTrigger(_onHurtHash);
	}

	private void TriggerRegenAnimation() {
		_animator.SetTrigger(_onRegenHash);
	}
}
