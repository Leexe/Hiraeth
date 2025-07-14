using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour {
	[SerializeField] private HealthManager _healthManager;
	[SerializeField] private Image _healthSlider;

	private void Start() {
		_healthManager.OnDamage.AddListener(ChangeSlider);
		_healthSlider.fillAmount = 1;
	}

	private void OnDestroy() {
		_healthManager.OnDamage.RemoveListener(ChangeSlider);
	}

	private void ChangeSlider(float health, float maxHealth) {
		_healthSlider.fillAmount = _healthManager.GetHealth / _healthManager.GetMaxHealth;
	}
}
