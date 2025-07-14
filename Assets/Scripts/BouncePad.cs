using UnityEngine;

public class BouncePad : MonoBehaviour {
	[Header("References")]
	[SerializeField] private LayerMask playerLayer;

	[Header("Bounce Pad Data")]
	[SerializeField] private float bouncePadForce = 10f;

	private void OnTriggerEnter(Collider other) {
		if (((1 << other.gameObject.layer) & playerLayer) != 0) {
			MyCharacterController characterController = other.gameObject.GetComponent<MyCharacterController>();
			characterController.ZeroAndAddVertVelocity(bouncePadForce);
			characterController.ResetJumps();
			characterController.ResetDashes();
			characterController.OnBouncePadTaken();
			PlayBouncePadSound();
		}
	}

	private void PlayBouncePadSound() {
		AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.BouncePad_sfx, gameObject);
	}
}
