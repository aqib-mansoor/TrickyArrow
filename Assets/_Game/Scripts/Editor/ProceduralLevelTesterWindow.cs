using UnityEngine;
using UnityEditor;
using _Game;

namespace _Game.Editor
{
    public class ProceduralLevelTesterWindow : EditorWindow
    {
        private int _testLevelNumber = 1;
        private Vector2 _scrollPos;
        private string _generationSummary = "";

        [MenuItem("Tools/TrickyArrow/Procedural Level Tester")]
        public static void ShowWindow()
        {
            GetWindow<ProceduralLevelTesterWindow>("Level Tester");
        }

        private void OnGUI()
        {
            GUILayout.Label("Procedural Level Generator & Difficulty Tester", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _testLevelNumber = EditorGUILayout.IntSlider("Test Level Number", _testLevelNumber, 1, 500);

            var config = ProceduralLevelGenerator.GetConfigForLevel(_testLevelNumber);

            EditorGUILayout.HelpBox(
                $"Level {_testLevelNumber} Metrics:\n" +
                $"• Grid Size: {config.gridWidth} x {config.gridHeight}\n" +
                $"• Arrow Count: {config.arrowCount}\n" +
                $"• Max Bends: {config.maxBendsPerArrow}\n" +
                $"• Time Limit: {config.timeLimit:F0}s",
                MessageType.Info
            );

            EditorGUILayout.Space();

            if (GUILayout.Button("Simulate Generation for this Level", GUILayout.Height(32)))
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                var paths = ProceduralLevelGenerator.GeneratePuzzle(_testLevelNumber, config);
                sw.Stop();

                _generationSummary = $"Generated Level {_testLevelNumber} in {sw.ElapsedMilliseconds} ms.\n" +
                                     $"Successfully built {paths.Count} arrow paths:\n";

                for (int i = 0; i < paths.Count; i++)
                {
                    _generationSummary += $"  [Arrow {i + 1}] Points: {paths[i].points.Count}, Exit Direction: {paths[i].headDirection}\n";
                }
            }

            if (GUILayout.Button("Batch Test First 100 Levels", GUILayout.Height(28)))
            {
                int successCount = 0;
                int totalArrows = 0;
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                for (int lvl = 1; lvl <= 100; lvl++)
                {
                    var cfg = ProceduralLevelGenerator.GetConfigForLevel(lvl);
                    var paths = ProceduralLevelGenerator.GeneratePuzzle(lvl, cfg);
                    if (paths.Count > 0)
                    {
                        successCount++;
                        totalArrows += paths.Count;
                    }
                }
                sw.Stop();

                _generationSummary = $"[BATCH TEST COMPLETE]\n" +
                                     $"Levels Tested: 100\n" +
                                     $"Successfully Generated: {successCount}/100\n" +
                                     $"Total Arrows Built: {totalArrows}\n" +
                                     $"Total Time Elapsed: {sw.ElapsedMilliseconds} ms (Avg: {(sw.ElapsedMilliseconds / 100f):F2} ms/level)";
            }

            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(_generationSummary))
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(220));
                EditorGUILayout.TextArea(_generationSummary, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
