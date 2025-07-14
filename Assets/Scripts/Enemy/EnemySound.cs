using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class EnemySound : MonoBehaviour {
	[Header("References")]
	[SerializeField] private HealthManager _healthManager;
	[SerializeField] private EnemyController _enemyController;

	private StudioEventEmitter _idleEmitter;

	private void Start() {
		_idleEmitter = AudioManager.Instance.InitalizeEventEmitter(FMODEvents.Instance.EnemyIdle_sfx, gameObject);
		_healthManager.OnDeath.AddListener(PlayEnemyDeathSound);
		_enemyController.OnAttack.AddListener(PlayEnemyAttackSound);
	}

	private void OnDestroy() {
		_healthManager.OnDeath.RemoveListener(PlayEnemyDeathSound);
	}

	private void RemoveIdleAudioOnDisable() {
		_idleEmitter.Stop();
	}

	private void PlayEnemyAttackSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.EnemyAttack_sfx, gameObject);
	}

	private void PlayEnemyDeathSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.EnemyDeath_sfx, gameObject);
	}
}
