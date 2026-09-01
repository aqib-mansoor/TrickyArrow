using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class FailPanel : UIPanel
    {
        [SerializeField] private Image _failIcon;
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private UIRootController _uiRoot;

        private Tween _iconTween;

		private void Awake()
		{
			if (_restartButton != null)
            {
                _restartButton.BindOnClick(this, OnRestartClicked);
                if (_restartButton.GetComponent<UIButtonPressEffect>() == null)
                {
                    _restartButton.gameObject.AddComponent<UIButtonPressEffect>();
                }
            }
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
            _iconTween?.Kill();
			// Auto-unsubscribe handled by ButtonExtensions
		}

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            if (_failIcon != null)
            {
                _iconTween?.Kill();
                _failIcon.transform.localScale = Vector3.one * 0.9f;
                _iconTween = _failIcon.transform.DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true)
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
        }

        public void Setup(int rewardedCoins, UIRootController uiRoot)
        {
            if (_coinText != null) _coinText.text = rewardedCoins.ToString();
            _uiRoot = uiRoot;
        }

        private void OnRestartClicked()
        {
			if (_uiRoot != null) _uiRoot.OnRestartConfirmed();
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}
