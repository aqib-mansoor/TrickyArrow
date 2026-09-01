using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI
{
    public class HeartUI : MonoBehaviour
    {
        [Header("Heart Sprites")]
        [SerializeField] private Sprite _redHeartSprite;
        [SerializeField] private Sprite _grayHeartSprite;

        [Header("Colors (TrickyArrow Theme)")]
        [SerializeField] private Color _activeColor = new Color(0.9843f, 0.4431f, 0.5216f, 1f); // #FB7185
        [SerializeField] private Color _inactiveColor = new Color(0.2000f, 0.2549f, 0.3333f, 1f); // #334155

        [Header("Image Component")]
        [SerializeField] private Image _heartImage;

        private bool _isActive = true;
        private bool _isInitialized = false;
        private Tween _scaleTween;
        private Tween _colorTween;
        private Vector3 _originalScale = Vector3.one;

        public void SetActive(bool active)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (_heartImage == null) return;

            bool wasActive = _isActive;
            _isActive = active;

            _scaleTween?.Kill();
            _colorTween?.Kill();

            if (active)
            {
                if (_redHeartSprite != null)
                {
                    _heartImage.sprite = _redHeartSprite;
                }
                _heartImage.color = _activeColor;
                transform.localScale = _originalScale;
            }
            else
            {
                if (wasActive)
                {
                    // Tactile life loss animation: scale pop (1.2x), punch, and smooth transition to inactive
                    Sequence seq = DOTween.Sequence()
                        .SetUpdate(true)
                        .SetAutoKill(true)
                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

                    seq.Append(transform.DOScale(_originalScale * 1.22f, 0.08f).SetEase(Ease.OutQuad));
                    seq.Append(transform.DOShakePosition(0.10f, strength: new Vector3(4f, 4f, 0), vibrato: 15));
                    seq.Join(transform.DOScale(_originalScale * 0.92f, 0.10f).SetEase(Ease.InQuad));
                    seq.Append(transform.DOScale(_originalScale, 0.06f).SetEase(Ease.OutBack));

                    if (_grayHeartSprite != null)
                    {
                        _heartImage.sprite = _grayHeartSprite;
                    }
                    _colorTween = _heartImage.DOColor(_inactiveColor, 0.18f)
                        .SetUpdate(true)
                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

                    _scaleTween = seq;
                }
                else
                {
                    if (_grayHeartSprite != null)
                    {
                        _heartImage.sprite = _grayHeartSprite;
                    }
                    _heartImage.color = _inactiveColor;
                    transform.localScale = _originalScale;
                }
            }
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            if (_heartImage == null)
            {
                _heartImage = GetComponent<Image>();
            }

            _originalScale = transform.localScale;
            if (_originalScale == Vector3.zero)
            {
                _originalScale = Vector3.one;
            }

            if (_heartImage == null)
            {
                Debug.LogWarning($"{name}: Image component is not found. Please assign it in Inspector.", this);
            }

            _isInitialized = true;
            SetActive(true);
        }

        private void OnDestroy()
        {
            _scaleTween?.Kill();
            _colorTween?.Kill();
        }
    }
}
