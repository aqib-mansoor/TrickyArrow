using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;

namespace SerapKeremGameKit._UI
{
    /// <summary>
    /// Premium animated startup splash screen for Tricky Arrow.
    /// Displays game logo with a smooth scale/glow pulse, animated title, loading progress indicator, and seamless fade-out into Level 1.
    /// </summary>
    public class SplashScreenPanel : UIPanel
    {
        [Header("Splash Elements")]
        [SerializeField] private Image _logoImage;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;
        [SerializeField] private Image _progressBar;
        [SerializeField] private CanvasGroup _splashContentGroup;

        [Header("Timing")]
        [SerializeField] private float _splashDuration = 2.0f;
        [SerializeField] private string _splashSoundKey = "ui_open";

        private static bool s_hasShownSplashThisSession = false;

        public bool ShouldShow => !s_hasShownSplashThisSession;

        public void PlaySplash(System.Action onComplete)
        {
            s_hasShownSplashThisSession = true;
            gameObject.SetActive(true);

            StartCoroutine(SplashRoutine(onComplete));
        }

        private IEnumerator SplashRoutine(System.Action onComplete)
        {
            if (canvasGroup != null) canvasGroup.alpha = 1f;

            // Load app icon if logoImage sprite not assigned
            if (_logoImage != null && _logoImage.sprite == null)
            {
                Sprite iconSprite = Resources.Load<Sprite>("Sprites/AppIcon");
                if (iconSprite == null) iconSprite = Resources.Load<Sprite>("AppIcon");
                if (iconSprite != null) _logoImage.sprite = iconSprite;
            }

            if (_logoImage != null)
            {
                _logoImage.transform.localScale = Vector3.one * 0.7f;
                _logoImage.transform
                    .DOScale(Vector3.one, 0.6f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
            }

            if (_titleText != null)
            {
                _titleText.transform.localScale = Vector3.one * 0.85f;
                _titleText.transform
                    .DOScale(Vector3.one, 0.6f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
            }

            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_splashSoundKey))
            {
                AudioManager.Instance.Play(_splashSoundKey);
            }
            if (HapticManager.IsInitialized)
            {
                HapticManager.Instance.Play(HapticType.Selection);
            }

            // Animate progress bar fill
            if (_progressBar != null)
            {
                _progressBar.fillAmount = 0f;
                _progressBar.DOFillAmount(1f, _splashDuration * 0.8f).SetEase(Ease.InOutSine).SetUpdate(true);
            }

            yield return new WaitForSecondsRealtime(_splashDuration);

            // Smooth fade out
            if (canvasGroup != null)
            {
                yield return canvasGroup.DOFade(0f, 0.45f).SetEase(Ease.InQuad).SetUpdate(true).WaitForCompletion();
            }

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
