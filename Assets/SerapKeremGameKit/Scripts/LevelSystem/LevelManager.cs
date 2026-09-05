using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Singletons;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Utilities;
using _Game;

namespace SerapKeremGameKit._Managers
{
    [DefaultExecutionOrder(-2)]
    public class LevelManager : MonoSingleton<LevelManager>
    {
        #region Properties & Data Access

        private const string ProgressKey = PreferencesKeys.ProgressData;
        public int ActiveLevelNumber
        {
            get => PlayerPrefs.GetInt(ProgressKey, 1);
            set { PlayerPrefs.SetInt(ProgressKey, value); SaveUtility.SaveImmediate(); }
        }

        [Header("Procedural Level Settings")]
        [Tooltip("When enabled, all levels are generated procedurally with mathematically guaranteed solutions and infinite scaling.")]
        [SerializeField] private bool _useProceduralInfiniteLevels = true;
        [SerializeField] private Level _proceduralBaseTemplate;

        [Title("Level Collections")]
        [ListDrawerSettings(Draggable = true, AlwaysExpanded = false)]
        [FormerlySerializedAs("_gameplayLevels")]
        [SerializeField] private Level[] _levels;

        public Level ActiveLevelInstance { get; private set; }
        public int ProcessedLevelIndex { get; private set; }

        // Public accessors for external systems
        public Level[] GameplayLevels => _levels;
        public int GameplayLevelCount => _levels != null ? _levels.Length : 0;
        public bool UseProceduralInfiniteLevels => _useProceduralInfiniteLevels;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            PerformInitialValidation();
        }

        void Start()
        {
            StartCurrentLevelInstance();
        }

        public void StartCurrentLevelInstance()
        {
            ConfigureEnvironment();
            LoadCurrentLevel();
        }

        #endregion

        #region Core Level Management

        public void LoadCurrentLevel()
        {
            int currentProgress = ActiveLevelNumber;
            ProcessedLevelIndex = currentProgress;

            if (_useProceduralInfiniteLevels)
            {
                InstantiateAndBeginProcedural(currentProgress);
            }
            else
            {
                var selection = ComputeLevelSelection();
                ProcessedLevelIndex = selection.targetIndex;
                InstantiateAndBegin(selection.selectedLevel);
            }
        }

        private void InstantiateAndBeginProcedural(int levelNumber)
        {
            Level baseTemplate = _proceduralBaseTemplate;
            if (baseTemplate == null)
            {
                baseTemplate = Resources.Load<Level>("Levels/Level_Base");
            }

            if (baseTemplate != null)
            {
                ActiveLevelInstance = Instantiate(baseTemplate);
            }
            else
            {
                GameObject newLevelGo = new GameObject($"Level {levelNumber}");
                ActiveLevelInstance = newLevelGo.AddComponent<ProceduralLevel>();
            }

            // Configure Procedural component
            ProceduralLevel procLevel = ActiveLevelInstance.GetComponent<ProceduralLevel>();
            if (procLevel == null)
            {
                procLevel = ActiveLevelInstance.gameObject.AddComponent<ProceduralLevel>();
            }
            procLevel.SetLevelNumber(levelNumber);

            ActiveLevelInstance.Load();
            Time.timeScale = 1f;
            if (SerapKeremGameKit._InputSystem.InputHandler.Instance != null)
            {
                SerapKeremGameKit._InputSystem.InputHandler.Instance.UnlockInput();
            }
            StateManager.Instance.SetLoading();
            StartLevel();
        }

        private (Level selectedLevel, int targetIndex) ComputeLevelSelection()
        {
            int currentProgress = ActiveLevelNumber;
            return ResolveGameplaySelection(currentProgress);
        }

        private (Level selectedLevel, int targetIndex) ResolveGameplaySelection(int adjustedProgress)
        {
            if (_levels == null || _levels.Length == 0)
            {
                Level baseTemplate = Resources.Load<Level>("Levels/Level_Base");
                return (baseTemplate, 1);
            }

            int totalGameplayLevels = _levels.Length;
            int calculatedIndex = WrapIndex(adjustedProgress, totalGameplayLevels);

            return (_levels[calculatedIndex - 1], calculatedIndex);
        }

        private int WrapIndex(int value, int wrapLimit)
        {
            if (wrapLimit <= 0) return 1;
            int remainder = value % wrapLimit;
            return remainder == 0 ? wrapLimit : remainder;
        }

        private void InstantiateAndBegin(Level targetLevel)
        {
            ActiveLevelInstance = Instantiate(targetLevel);
            ActiveLevelInstance.Load();
            Time.timeScale = 1f;
            if (SerapKeremGameKit._InputSystem.InputHandler.Instance != null)
            {
                SerapKeremGameKit._InputSystem.InputHandler.Instance.UnlockInput();
            }
            StateManager.Instance.SetLoading();
            StartLevel();
        }

        #endregion

        #region Level Control Methods

        public void StartLevel()
        {
            ActiveLevelInstance.Play();
            StateManager.Instance.SetOnStart();
        }

        public void RetryLevel()
        {
            TerminateCurrentLevel();
            if (_useProceduralInfiniteLevels)
            {
                InstantiateAndBeginProcedural(ActiveLevelNumber);
            }
            else if (_levels != null && _levels.Length > 0)
            {
                var retryTarget = _levels[Mathf.Clamp(ProcessedLevelIndex - 1, 0, _levels.Length - 1)];
                InstantiateAndBegin(retryTarget);
            }
            else
            {
                InstantiateAndBeginProcedural(ActiveLevelNumber);
            }
        }

        public void RestartLevel()
        {
            StateManager.Instance.SetOnRestart();
            RetryLevel();
        }

        public void CleanCurrentLevel()
        {
            TerminateCurrentLevel();
        }

        public void IncreaseLevelNumber()
        {
            TerminateCurrentLevel();
            ActiveLevelNumber++;
        }

        private void TerminateCurrentLevel()
        {
            if (ActiveLevelInstance != null)
                Destroy(ActiveLevelInstance.gameObject);
        }

        #endregion

        #region Game Result Handlers
        [Button("Test LevelWin")]
        public void Win()
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnWin();
        }

        [Button("Test LevelWin")]
        public void Win(int moveCount)
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnWin();
        }

        [Button("Test LevelLose")]
        public void Lose()
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnLose();
        }

        private bool ValidateGameStateForEvents()
        {
            return StateManager.Instance.CurrentState == GameState.OnStart;
        }

        #endregion

        #region Utility & Validation Methods

        private void PerformInitialValidation()
        {
            if (!_useProceduralInfiniteLevels && (_levels == null || _levels.Length == 0))
                TraceLogger.LogWarning($"{name}: Levels array is not configured.", this);
        }

        private void ConfigureEnvironment()
        {
#if UNITY_EDITOR
            CleanupExistingLevelsInEditor();
#endif
        }

#if UNITY_EDITOR
        private void CleanupExistingLevelsInEditor()
        {
            var existingLevelInstances = FindObjectsByType<Level>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var levelInstance in existingLevelInstances)
                levelInstance.gameObject.SetActive(false);
        }
#endif

        #endregion

        public Level GetLevelByNumber(int levelNumber)
        {
            if (_useProceduralInfiniteLevels) return null;

            int gameplayIndex = levelNumber;
            if (_levels == null || gameplayIndex <= 0 || gameplayIndex > _levels.Length) return null;

            return _levels[gameplayIndex - 1];
        }

        #region Utility & Validation Methods
        public Level GetCurrentLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber);
        }

        public Level GetNextLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 1) ?? GetLevelByNumber(1);
        }

        public Level GetNextestLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 2) ?? GetLevelByNumber(1);
        }

        public Level GetFinalLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 3) ?? GetLevelByNumber(1);
        }

        public Level GetFinalNextLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 4) ?? GetLevelByNumber(1);
        }
        #endregion
    }
}