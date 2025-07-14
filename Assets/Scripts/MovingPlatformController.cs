using Sirenix.OdinInspector;
using KinematicCharacterController;
using UnityEngine;
using PrimeTween;

public class MovingPlatformController : MonoBehaviour, IMoverController {
	[Header("References")]
	[SerializeField] private bool _hasPhysics = true;
	[ShowIf("_hasPhysics")]
	[SerializeField] private PhysicsMover _motor;

	[Header("Position Tweening")]
	[SerializeField] private bool _tweenPos;
	[ShowIf("_tweenPos")]
	[SerializeField] private Vector3 _endPos;
	[ShowIf("_tweenPos")]
	[SerializeField] private float _posTweenDuration = 3f;
	private Vector3 _originalPos;

	[Header("Rotation Tweening")]
	[SerializeField] private bool _tweenRot;
	[ShowIf("_tweenRot")]
	[SerializeField] private Quaternion _endRot;
	[ShowIf("_tweenRot")]
	[SerializeField] private float _rotTweenDuration = 3f;
	private Quaternion _originalRot;

	private Vector3 positionToTween;
	private Quaternion rotationToTween;

	private void Start() {
		_originalPos = transform.position;
		_originalRot = transform.rotation;
		positionToTween = _originalPos;
		rotationToTween = _originalRot;

		if (_tweenPos) {
			TweenPosition();
		}

		if (_tweenRot) {
			TweenRotation();
		}

		_motor.MoverController = this;
	}

	public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime) {
		goalPosition = positionToTween;
		goalRotation = rotationToTween;
	}

	private void TweenPosition() {
		Tween.Custom(_originalPos, _endPos, _posTweenDuration, newVal => positionToTween = newVal, Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
	}

	private void TweenRotation() {
		Tween.Custom(_originalRot, _endRot, _posTweenDuration, newVal => rotationToTween = newVal, Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
	}
}
