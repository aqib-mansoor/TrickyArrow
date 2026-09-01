using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SerapKeremGameKit._UI
{
    /// <summary>
    /// Provides tactile scale punch feedback on UI buttons when pressed.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float _pressedScale = 0.94f;
        [SerializeField] private float _downDuration = 0.08f;
        [SerializeField] private float _upDuration = 0.12f;
        [SerializeField] private Ease _downEase = Ease.OutQuad;
        [SerializeField] private Ease _upEase = Ease.OutBack;

        private Vector3 _originalScale = Vector3.one;
        private Tween _tween;
        private bool _isPressed = false;

        private void Awake()
        {
            _originalScale = transform.localScale;
            if (_originalScale == Vector3.zero)
                _originalScale = Vector3.one;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale * _pressedScale, _downDuration)
                .SetEase(_downEase)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isPressed) return;
            _isPressed = false;
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale, _upDuration)
                .SetEase(_upEase)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isPressed) return;
            _isPressed = false;
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale, _upDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void OnDisable()
        {
            _isPressed = false;
            _tween?.Kill();
            transform.localScale = _originalScale;
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
