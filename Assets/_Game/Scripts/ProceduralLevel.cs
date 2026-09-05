using System.Collections.Generic;
using UnityEngine;
using _Game.Data;
using _Game.Line;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Camera;
using SerapKeremGameKit._Logging;

namespace _Game
{
    /// <summary>
    /// Level component that dynamically builds a procedural puzzle board upon loading.
    /// Handles line instantiation, pool assignment, background sizing, and camera framing.
    /// </summary>
    public class ProceduralLevel : Level
    {
        [Header("Procedural Setup")]
        [SerializeField] private GameObject _linePrefab;
        [SerializeField] private MeshRenderer _backgroundRenderer;
        [SerializeField] private Transform _generatedLinesParent;
        [SerializeField] private Vector3ArrayPool _arrayPool;

        private int _levelNumber = 1;

        public void SetLevelNumber(int levelNumber)
        {
            _levelNumber = levelNumber;
        }

        public override void Load()
        {
            gameObject.SetActive(true);

            // Apply visual palette for this level
            GameplayVisualThemeSO.Instance.ApplyThemeForLevel(_levelNumber);

            BuildProceduralBoard();

            base.Load();
        }

        private void BuildProceduralBoard()
        {
            if (_generatedLinesParent == null)
            {
                Transform existingParent = transform.Find("LINES");
                if (existingParent != null)
                {
                    _generatedLinesParent = existingParent;
                }
                else
                {
                    GameObject go = new GameObject("LINES");
                    go.transform.SetParent(transform, false);
                    _generatedLinesParent = go.transform;
                }
            }

            // Clear any previous lines
            for (int i = _generatedLinesParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_generatedLinesParent.GetChild(i).gameObject);
            }

            if (_linePrefab == null)
            {
                _linePrefab = Resources.Load<GameObject>("Line/Line (1)");
            }

            if (_arrayPool == null)
            {
                _arrayPool = GetComponentInChildren<Vector3ArrayPool>(true);
            }

            if (_backgroundRenderer == null)
            {
                Transform bgTransform = transform.Find("Background");
                if (bgTransform != null)
                {
                    _backgroundRenderer = bgTransform.GetComponent<MeshRenderer>();
                }
            }

            // Generate puzzle layout
            var config = ProceduralLevelGenerator.GetConfigForLevel(_levelNumber);
            var paths = ProceduralLevelGenerator.GeneratePuzzle(_levelNumber, config);

            // Center the grid around world (0,0)
            float halfW = (config.gridWidth - 1) * 0.5f;
            float halfH = (config.gridHeight - 1) * 0.5f;

            foreach (var arrow in paths)
            {
                if (arrow.points == null || arrow.points.Count < 2)
                    continue;

                GameObject lineObj = null;
                if (_linePrefab != null)
                {
                    lineObj = Instantiate(_linePrefab, _generatedLinesParent);
                }
                else
                {
                    lineObj = new GameObject("Line");
                    lineObj.transform.SetParent(_generatedLinesParent, false);
                }

                Line lineComp = lineObj.GetComponent<Line>();
                LineRenderer lr = lineObj.GetComponent<LineRenderer>();

                if (lr != null)
                {
                    lr.positionCount = arrow.points.Count;
                    for (int i = 0; i < arrow.points.Count; i++)
                    {
                        Vector2Int pt = arrow.points[i];
                        Vector3 worldPos = new Vector3(pt.x - halfW, pt.y - halfH, 0f);
                        lr.SetPosition(i, worldPos);
                    }
                }

                // Update LineAnimation array pool if present
                LineAnimation anim = lineObj.GetComponent<LineAnimation>();
                if (anim != null && _arrayPool != null)
                {
                    anim.Initialize(lr, _arrayPool);
                }
            }

            // Adjust Background quad to cover grid
            if (_backgroundRenderer != null)
            {
                float bgScaleX = Mathf.Max(config.gridWidth + 3f, 6f);
                float bgScaleY = Mathf.Max(config.gridHeight + 3f, 6f);
                _backgroundRenderer.transform.localScale = new Vector3(bgScaleX, bgScaleY, 1f);
                _backgroundRenderer.transform.localPosition = new Vector3(0f, 0f, 1f);

                if (_backgroundRenderer.material != null)
                {
                    _backgroundRenderer.material.color = GameplayVisualThemeSO.Instance.BackgroundColor;
                }
            }
        }
    }
}
