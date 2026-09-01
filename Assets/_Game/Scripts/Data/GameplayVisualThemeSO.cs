using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "GameplayVisualTheme", menuName = "TrickyArrow/GameplayVisualTheme", order = 0)]
    public class GameplayVisualThemeSO : ScriptableObject
    {
        [Header("Line State Colors")]
        [Tooltip("Idle line color on the dark board (#E2E8F0)")]
        [SerializeField] private Color _idleLineColor = new Color(0.886f, 0.910f, 0.941f, 1f);

        [Tooltip("Active/moving line color (#38BDF8)")]
        [SerializeField] private Color _activeLineColor = new Color(0.220f, 0.741f, 0.973f, 1f);

        [Tooltip("Collision failure line color (#F43F5E)")]
        [SerializeField] private Color _failureLineColor = new Color(0.957f, 0.247f, 0.369f, 1f);

        [Tooltip("Successful completion line color (#10B981)")]
        [SerializeField] private Color _successLineColor = new Color(0.063f, 0.725f, 0.506f, 1f);

        [Header("Board Surface Colors")]
        [Tooltip("Gameplay background slate (#0F172A)")]
        [SerializeField] private Color _backgroundColor = new Color(0.059f, 0.090f, 0.165f, 1f);

        [Header("Transition Settings")]
        [Tooltip("Duration in seconds that failure color is shown upon collision")]
        [SerializeField] private float _failureFlashDuration = 0.5f;

        [Tooltip("Duration in seconds for completion success highlight")]
        [SerializeField] private float _completionFadeDuration = 0.3f;

        public Color IdleLineColor => _idleLineColor;
        public Color ActiveLineColor => _activeLineColor;
        public Color FailureLineColor => _failureLineColor;
        public Color SuccessLineColor => _successLineColor;
        public Color BackgroundColor => _backgroundColor;
        public float FailureFlashDuration => _failureFlashDuration;
        public float CompletionFadeDuration => _completionFadeDuration;

        private static GameplayVisualThemeSO _instance;
        public static GameplayVisualThemeSO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameplayVisualThemeSO>("Data/GameplayVisualTheme");
                    if (_instance == null)
                    {
                        _instance = Resources.Load<GameplayVisualThemeSO>("GameplayVisualTheme");
                    }
                    if (_instance == null)
                    {
                        _instance = CreateInstance<GameplayVisualThemeSO>();
                    }
                }
                return _instance;
            }
        }
    }
}
