using UnityEngine;
using PrimeTween;

public class HandAnimations : MonoBehaviour {
    [SerializeField] private RectTransform _handRectTransform;

    [Header("Idle")]
    [SerializeField] private TweenSettings<Vector2> handIdleAnimation;

    private void Start() {
        Tween.UIAnchoredPosition(_handRectTransform, handIdleAnimation);
    }
}
