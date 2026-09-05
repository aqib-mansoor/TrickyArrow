using System.Collections.Generic;
using UnityEngine;
using _Game.Data;
using _Game.Line;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Managers;

namespace _Game
{
    /// <summary>
    /// Generates infinite, guaranteed-solvable arrow puzzle levels with dynamic difficulty scaling.
    /// Uses reverse puzzle assembly so every board is mathematically verified to be solvable.
    /// </summary>
    public static class ProceduralLevelGenerator
    {
        public struct LevelConfigData
        {
            public int gridWidth;
            public int gridHeight;
            public int arrowCount;
            public int maxBendsPerArrow;
            public float timeLimit;
        }

        public struct ArrowPath
        {
            public List<Vector2Int> points; // Grid coordinates from tail to head
            public Vector2Int headDirection; // Final moving direction
        }

        public static LevelConfigData GetConfigForLevel(int levelNumber)
        {
            // Level 1-3: Small Tutorial/Warmup (3x3 to 4x4, 4-6 straight/1-bend arrows)
            // Level 4-10: Easy-Medium (5x5 to 6x6, 7-12 arrows, 1-2 bends)
            // Level 11-25: Challenging (7x7 to 8x8, 14-22 arrows, 2-3 bends)
            // Level 26-50: Expert (9x9 to 11x11, 24-38 arrows, 3-4 bends)
            // Level 51+: Master/Infinite (12x12 to 15x15, 40-60+ arrows, intricate labyrinths)

            int width = Mathf.Clamp(3 + (levelNumber / 4), 3, 14);
            int height = Mathf.Clamp(3 + (levelNumber / 4), 3, 14);

            int arrowCount = Mathf.Clamp(4 + Mathf.RoundToInt(levelNumber * 1.35f), 4, 65);
            int maxBends = Mathf.Clamp(1 + (levelNumber / 8), 1, 4);
            float timeLimit = Mathf.Clamp(45f + (arrowCount * 3.5f), 60f, 300f);

            return new LevelConfigData
            {
                gridWidth = width,
                gridHeight = height,
                arrowCount = arrowCount,
                maxBendsPerArrow = maxBends,
                timeLimit = timeLimit
            };
        }

        public static List<ArrowPath> GeneratePuzzle(int levelNumber, LevelConfigData config)
        {
            // Deterministic seed based on level number: Restarting produces the same puzzle, advancing gives a unique one
            Random.InitState(levelNumber * 7919 + 1337);

            int W = config.gridWidth;
            int H = config.gridHeight;

            // Grid cell occupancy: tracks placed segments
            HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
            List<ArrowPath> generatedArrows = new List<ArrowPath>();

            Vector2Int[] cardinalDirections = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            int targetArrows = config.arrowCount;
            int maxAttempts = targetArrows * 35;
            int attempts = 0;

            while (generatedArrows.Count < targetArrows && attempts < maxAttempts)
            {
                attempts++;

                // 1. Pick a random exit boundary point and an outward exit direction
                Vector2Int exitDir = cardinalDirections[Random.Range(0, 4)];
                Vector2Int headPos = GetRandomBoundaryCell(W, H, exitDir);

                if (occupiedCells.Contains(headPos))
                    continue;

                // 2. Trace arrow path backwards into the board
                List<Vector2Int> path = new List<Vector2Int>();
                path.Add(headPos);

                Vector2Int current = headPos;
                Vector2Int currentDir = -exitDir; // Step backwards

                int desiredBends = Random.Range(1, config.maxBendsPerArrow + 1);
                int segmentLength = Random.Range(2, Mathf.Max(3, Mathf.Min(W, H) - 1));
                int currentBends = 0;
                bool validPath = true;

                for (int s = 0; s < segmentLength; s++)
                {
                    Vector2Int next = current + currentDir;

                    if (next.x < 0 || next.x >= W || next.y < 0 || next.y >= H)
                        break;

                    if (occupiedCells.Contains(next))
                        break;

                    current = next;
                    path.Add(current);

                    // Chance to bend
                    if (currentBends < desiredBends && s >= 1 && Random.value < 0.45f)
                    {
                        Vector2Int[] perpendiculars = GetPerpendicularDirections(currentDir);
                        Vector2Int newDir = perpendiculars[Random.Range(0, 2)];
                        Vector2Int testNext = current + newDir;

                        if (testNext.x >= 0 && testNext.x < W && testNext.y >= 0 && testNext.y < H && !occupiedCells.Contains(testNext))
                        {
                            currentDir = newDir;
                            currentBends++;
                        }
                    }
                }

                if (path.Count >= 2)
                {
                    // Valid arrow constructed! Reverse path so points are [tail ... head]
                    path.Reverse();

                    foreach (var cell in path)
                    {
                        occupiedCells.Add(cell);
                    }

                    generatedArrows.Add(new ArrowPath
                    {
                        points = path,
                        headDirection = exitDir
                    });
                }
            }

            return generatedArrows;
        }

        private static Vector2Int GetRandomBoundaryCell(int W, int H, Vector2Int exitDir)
        {
            if (exitDir == Vector2Int.up)
                return new Vector2Int(Random.Range(0, W), H - 1);
            if (exitDir == Vector2Int.down)
                return new Vector2Int(Random.Range(0, W), 0);
            if (exitDir == Vector2Int.right)
                return new Vector2Int(W - 1, Random.Range(0, H));
            // left
            return new Vector2Int(0, Random.Range(0, H));
        }

        private static Vector2Int[] GetPerpendicularDirections(Vector2Int dir)
        {
            if (dir == Vector2Int.up || dir == Vector2Int.down)
            {
                return new Vector2Int[] { Vector2Int.left, Vector2Int.right };
            }
            return new Vector2Int[] { Vector2Int.up, Vector2Int.down };
        }
    }
}
