using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour {
	[SerializeField] private LayerMask _destroyLayer;
	[SerializeField] private LayerMask _targetLayer;
	[SerializeField] private LayerMask _bulletLayer;
	[SerializeField] private Rigidbody _rb;
	[SerializeField] private Image _image;
	[SerializeField] private float _bulletCollisionLifetime = 0.6f;
	[SerializeField] private float _bulletLifetime = 2.5f;

	private float _damage = 0f;
	private IEnumerator _lifetime;

	private void Start() {
		_lifetime = DestroyBullet();
		StartCoroutine(_lifetime);
	}

	public void InitiateBullet(float damage) {
		_damage = damage;
	}

	private void OnCollisionEnter(Collision collision) {
		// Hits the target layer
		if (((1 << collision.gameObject.layer) & _targetLayer) > 0) {
			collision.gameObject.GetComponent<HealthManager>().TakeDamage(_damage);

			// Deal with bullet lifetime
			StopCoroutine(_lifetime);
			_lifetime = DestroyBulletAfterCollision();
			DestroyBulletAfterCollision();
			StartCoroutine(_lifetime);
		}

		// Hits a layer that destroys the bullet
		if (((1 << collision.gameObject.layer) & _destroyLayer) > 0) {
			// Deal with bullet lifetime
			StopCoroutine(_lifetime);
			_lifetime = DestroyBulletAfterCollision();
			DestroyBulletAfterCollision();
			StartCoroutine(_lifetime);
		}

		// Play parry sound
		if (((1 << collision.gameObject.layer) & _bulletLayer) > 0) {
			AudioManager.Instance.PlayOneShotAtPos(FMODEvents.Instance.Parry_sfx, transform.position);
		}
	}

	private IEnumerator DestroyBullet() {
		yield return new WaitForSeconds(_bulletLifetime);
		Destroy(gameObject);
	}

	private IEnumerator DestroyBulletAfterCollision() {
		_rb.linearVelocity = Vector3.zero;
		_image.enabled = false;
		yield return new WaitForSeconds(_bulletCollisionLifetime);
		Destroy(gameObject);
	}
}
