using UnityEngine;

public class EnemyVisual : MonoBehaviour {
	[SerializeField] private HealthManager _healthManager;
	[SerializeField] private Animator _animator;

	private int _deathName = Animator.StringToHash("Death");

	private void Start() {
		_healthManager.OnDeath.AddListener(PlayDeathAnimation);
	}

	private void PlayDeathAnimation() {
		_animator.SetTrigger(_deathName);
	}
}
