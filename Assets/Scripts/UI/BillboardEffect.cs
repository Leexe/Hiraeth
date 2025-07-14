using UnityEngine;

public class BillboardEffect : MonoBehaviour {
	[Header("References")]
	[SerializeField] private RectTransform _rectTransform;
	[SerializeField] private bool _lockHorizontalRotation = false;

	private Camera _camera;

	private void Start() {
		_camera = Camera.main;
	}

	private void Update() {
		_rectTransform.transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
		if (_lockHorizontalRotation) {
			_rectTransform.transform.rotation = Quaternion.Euler(0f, _rectTransform.transform.rotation.eulerAngles.y, 0f);
		}
	}
}
