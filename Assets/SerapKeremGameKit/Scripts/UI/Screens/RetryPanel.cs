using UnityEngine;
using UnityEngine.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class RetryPanel : UIPanel
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private UIRootController _uiRoot;

		private void Awake()
		{
			if (_yesButton != null)
            {
                _yesButton.BindOnClick(this, OnYes);
                if (_yesButton.GetComponent<UIButtonPressEffect>() == null)
                {
                    _yesButton.gameObject.AddComponent<UIButtonPressEffect>();
                }
            }

			if (_noButton != null)
            {
                _noButton.BindOnClick(this, OnNo);
                if (_noButton.GetComponent<UIButtonPressEffect>() == null)
                {
                    _noButton.gameObject.AddComponent<UIButtonPressEffect>();
                }
            }
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			// Auto-unsubscribe handled by ButtonExtensions
		}

        private void OnYes()
        {
			if (_uiRoot != null) _uiRoot.OnRestartConfirmed();
        }

        private void OnNo()
        {
            Hide();
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}