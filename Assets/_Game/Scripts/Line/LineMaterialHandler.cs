using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Data;

namespace _Game.Line
{
    public class LineMaterialHandler : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private List<Component> _renderers = new List<Component>();

        [Header("Theme Settings")]
        [SerializeField] private GameplayVisualThemeSO _theme;

        [Header("Color Settings (Overrides if Theme is unassigned)")]
        [SerializeField] private Color _failureColor = new Color(0.957f, 0.247f, 0.369f, 1f); // #F43F5E
        [SerializeField] private float _failureColorDuration = 0.5f;

        private Dictionary<Component, Color> _originalColors = new Dictionary<Component, Color>();
        private Coroutine _colorResetCoroutine;

        public GameplayVisualThemeSO Theme
        {
            get
            {
                if (_theme == null)
                {
                    _theme = GameplayVisualThemeSO.Instance;
                }
                return _theme;
            }
            set => _theme = value;
        }

        private void Awake()
        {
            StoreOriginalColors();
            ApplyIdleThemeColor();
        }

        private void ApplyIdleThemeColor()
        {
            if (Theme != null)
            {
                SetColor(Theme.IdleLineColor);
                // Update cached original colors to the theme idle color
                List<Component> keys = new List<Component>(_originalColors.Keys);
                foreach (var key in keys)
                {
                    _originalColors[key] = Theme.IdleLineColor;
                }
            }
        }

        private void StoreOriginalColors()
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    StoreRendererColor(renderer);
                }
            }
        }

        private void StoreRendererColor(Component renderer)
        {
            if (renderer is LineRenderer lineRenderer)
            {
                if (lineRenderer.sharedMaterial != null)
                {
                    lineRenderer.material = new Material(lineRenderer.sharedMaterial);
                }
                
                if (lineRenderer.material != null)
                {
                    _originalColors[renderer] = lineRenderer.material.color;
                }
            }
            else if (renderer is SpriteRenderer spriteRenderer)
            {
                _originalColors[renderer] = spriteRenderer.color;
            }
        }

        public void SetActiveColor()
        {
            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
                _colorResetCoroutine = null;
            }

            Color activeColor = Theme != null ? Theme.ActiveLineColor : new Color(0.220f, 0.741f, 0.973f, 1f);
            SetColor(activeColor);
        }

        public void SetSuccessColor()
        {
            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
                _colorResetCoroutine = null;
            }

            Color successColor = Theme != null ? Theme.SuccessLineColor : new Color(0.063f, 0.725f, 0.506f, 1f);
            SetColor(successColor);
        }

        public void SetFailureColor()
        {
            Color failColor = Theme != null ? Theme.FailureLineColor : _failureColor;
            float failDuration = Theme != null ? Theme.FailureFlashDuration : _failureColorDuration;

            SetColor(failColor);

            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
            }

            _colorResetCoroutine = StartCoroutine(ResetColorAfterDelay(failDuration));
        }

        private IEnumerator ResetColorAfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            ResetToOriginalColors();
            _colorResetCoroutine = null;
        }

        public void ResetToOriginalColors()
        {
            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
                _colorResetCoroutine = null;
            }

            foreach (var kvp in _originalColors)
            {
                if (kvp.Key == null) continue;

                if (kvp.Key is LineRenderer lineRenderer && lineRenderer.material != null)
                {
                    lineRenderer.material.color = kvp.Value;
                }
                else if (kvp.Key is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = kvp.Value;
                }
            }
        }

        private void OnDestroy()
        {
            if (_colorResetCoroutine != null)
            {
                StopCoroutine(_colorResetCoroutine);
                _colorResetCoroutine = null;
            }
        }

        public void SetColor(Color color)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;

                if (renderer is LineRenderer lineRenderer)
                {
                    if (lineRenderer.material != null)
                    {
                        lineRenderer.material.color = color;
                    }
                }
                else if (renderer is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = color;
                }
            }
        }

        public void AddRenderer(Component renderer)
        {
            if (renderer != null && !_renderers.Contains(renderer))
            {
                _renderers.Add(renderer);
                StoreRendererColor(renderer);
                if (Theme != null)
                {
                    _originalColors[renderer] = Theme.IdleLineColor;
                    if (renderer is LineRenderer lr && lr.material != null)
                    {
                        lr.material.color = Theme.IdleLineColor;
                    }
                    else if (renderer is SpriteRenderer sr)
                    {
                        sr.color = Theme.IdleLineColor;
                    }
                }
            }
        }
    }
}

