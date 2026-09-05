using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Data
{
    [System.Serializable]
    public class ColorPalette
    {
        public string themeName;
        public Color idleLineColor = new Color(0.886f, 0.910f, 0.941f, 1f);
        public Color activeLineColor = new Color(0.220f, 0.741f, 0.973f, 1f);
        public Color failureLineColor = new Color(0.957f, 0.247f, 0.369f, 1f);
        public Color successLineColor = new Color(0.063f, 0.725f, 0.506f, 1f);
        public Color backgroundColor = new Color(0.059f, 0.090f, 0.165f, 1f);
    }

    [CreateAssetMenu(fileName = "GameplayVisualTheme", menuName = "TrickyArrow/GameplayVisualTheme", order = 0)]
    public class GameplayVisualThemeSO : ScriptableObject
    {
        [Header("Line State Colors (Default/Active Palette)")]
        [SerializeField] private Color _idleLineColor = new Color(0.886f, 0.910f, 0.941f, 1f);
        [SerializeField] private Color _activeLineColor = new Color(0.220f, 0.741f, 0.973f, 1f);
        [SerializeField] private Color _failureLineColor = new Color(0.957f, 0.247f, 0.369f, 1f);
        [SerializeField] private Color _successLineColor = new Color(0.063f, 0.725f, 0.506f, 1f);

        [Header("Board Surface Colors")]
        [SerializeField] private Color _backgroundColor = new Color(0.059f, 0.090f, 0.165f, 1f);

        [Header("Transition Settings")]
        [SerializeField] private float _failureFlashDuration = 0.5f;
        [SerializeField] private float _completionFadeDuration = 0.3f;

        [Header("Procedural Dynamic Themes")]
        [SerializeField] private List<ColorPalette> _palettes = new List<ColorPalette>()
        {
            new ColorPalette()
            {
                themeName = "Midnight Slate",
                idleLineColor = new Color(0.886f, 0.910f, 0.941f, 1f),
                activeLineColor = new Color(0.220f, 0.741f, 0.973f, 1f),
                failureLineColor = new Color(0.957f, 0.247f, 0.369f, 1f),
                successLineColor = new Color(0.063f, 0.725f, 0.506f, 1f),
                backgroundColor = new Color(0.059f, 0.090f, 0.165f, 1f)
            },
            new ColorPalette()
            {
                themeName = "Neon Cyberpunk",
                idleLineColor = new Color(0.95f, 0.95f, 0.98f, 1f),
                activeLineColor = new Color(0.98f, 0.22f, 0.76f, 1f), // Neon pink
                failureLineColor = new Color(1.0f, 0.20f, 0.20f, 1f),
                successLineColor = new Color(0.18f, 0.98f, 0.55f, 1f), // Neon mint
                backgroundColor = new Color(0.08f, 0.04f, 0.16f, 1f) // Deep purple
            },
            new ColorPalette()
            {
                themeName = "Emerald Forest",
                idleLineColor = new Color(0.85f, 0.95f, 0.90f, 1f),
                activeLineColor = new Color(0.15f, 0.85f, 0.45f, 1f), // Vibrant Emerald
                failureLineColor = new Color(0.95f, 0.30f, 0.25f, 1f),
                successLineColor = new Color(0.30f, 0.95f, 0.65f, 1f),
                backgroundColor = new Color(0.03f, 0.12f, 0.09f, 1f) // Deep Forest
            },
            new ColorPalette()
            {
                themeName = "Sunset Amber",
                idleLineColor = new Color(0.98f, 0.94f, 0.88f, 1f),
                activeLineColor = new Color(0.98f, 0.58f, 0.15f, 1f), // Warm Amber
                failureLineColor = new Color(0.95f, 0.20f, 0.35f, 1f),
                successLineColor = new Color(0.30f, 0.85f, 0.40f, 1f),
                backgroundColor = new Color(0.14f, 0.07f, 0.08f, 1f) // Warm dark maroon
            },
            new ColorPalette()
            {
                themeName = "Deep Ocean",
                idleLineColor = new Color(0.85f, 0.92f, 0.98f, 1f),
                activeLineColor = new Color(0.00f, 0.82f, 0.95f, 1f), // Cyan glow
                failureLineColor = new Color(0.95f, 0.25f, 0.40f, 1f),
                successLineColor = new Color(0.10f, 0.80f, 0.65f, 1f),
                backgroundColor = new Color(0.02f, 0.08f, 0.15f, 1f) // Abyssal Blue
            }
        };

        public Color IdleLineColor => _idleLineColor;
        public Color ActiveLineColor => _activeLineColor;
        public Color FailureLineColor => _failureLineColor;
        public Color SuccessLineColor => _successLineColor;
        public Color BackgroundColor => _backgroundColor;
        public float FailureFlashDuration => _failureFlashDuration;
        public float CompletionFadeDuration => _completionFadeDuration;
        public IReadOnlyList<ColorPalette> Palettes => _palettes;

        public void ApplyThemeForLevel(int levelNumber)
        {
            if (_palettes != null && _palettes.Count > 0)
            {
                int paletteIndex = ((levelNumber - 1) / 3) % _palettes.Count;
                var p = _palettes[paletteIndex];
                _idleLineColor = p.idleLineColor;
                _activeLineColor = p.activeLineColor;
                _failureLineColor = p.failureLineColor;
                _successLineColor = p.successLineColor;
                _backgroundColor = p.backgroundColor;
            }
        }

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
