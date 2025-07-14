using UnityEngine;

public class Boundary : MonoBehaviour {
	private enum BoundaryType {
		lose = 0,
		win = 1,
		hurt = 2,
	}

	[SerializeField] private LayerMask _playerLayermask;
	[SerializeField] private BoundaryType _boundaryType;

	private bool _playerIsInsideCollider = false;
	private HealthManager _playerHealthManger;

	private void Update() {
		if (_boundaryType == BoundaryType.hurt && _playerIsInsideCollider) {
			_playerHealthManger.TakeDamage(1);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (((1 << other.gameObject.layer) & _playerLayermask) > 0) {
			if (_boundaryType == BoundaryType.lose) {
				GameManager.Instance.LoseGame();
			}
			else if (_boundaryType == BoundaryType.hurt) {
				_playerHealthManger = other.gameObject.GetComponent<HealthManager>();
				_playerHealthManger.TakeDamage(1);
				_playerIsInsideCollider = true;
			}
			else {
				GameManager.Instance.WinGame();
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (((1 << other.gameObject.layer) & _playerLayermask) > 0) {
			if (_boundaryType == BoundaryType.hurt) {
				_playerIsInsideCollider = false;
			}
		}
	}
}
