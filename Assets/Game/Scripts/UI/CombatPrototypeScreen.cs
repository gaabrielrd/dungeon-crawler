using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class CombatPrototypeScreen : UIScreen
    {
        private const float EnemyTurnDelaySeconds = 0.45f;

        [SerializeField] private ScreenNavigator navigator;
        [SerializeField] private Button backToMainMenuButton;
        [SerializeField] private Sprite[] playerPortraits;
        [SerializeField] private Sprite[] enemyPortraits;
        [SerializeField] private TestCombatDataProvider testData;

        private CombatController _combatController;
        private CombatFormationState _formation;
        private IEventBus _eventBus;
        private IDungeonRunService _dungeonRunService;
        private bool _subscribedToEvents;
        private bool _awaitingTargetSelection;
        private bool _isUiReady;
        private bool _isCombatInitialized;
        private Coroutine _enemyTurnRoutine;
        private readonly System.Random _enemyTargetRandom = new System.Random();

        private RectTransform _runtimeRoot;
        private Text _turnText;
        private Text _statusText;
        private Text _resultText;
        private Button _basicAttackButton;
        private Button _nextFloorButton;
        private readonly List<CombatantSlotView> _playerSlots = new List<CombatantSlotView>();
        private readonly List<CombatantSlotView> _enemySlots = new List<CombatantSlotView>();
        private readonly Dictionary<CombatantState, CombatantSlotView> _slotsByCombatant = new Dictionary<CombatantState, CombatantSlotView>();

        private static readonly Color EmptySlotColor = new Color(0.15f, 0.16f, 0.19f, 0.92f);
        private static readonly Color PlayerSlotColor = new Color(0.18f, 0.33f, 0.52f, 0.95f);
        private static readonly Color EnemySlotColor = new Color(0.52f, 0.20f, 0.20f, 0.95f);
        private static readonly Color HighlightColor = new Color(0.95f, 0.72f, 0.28f, 0.96f);
        private static readonly Color ActiveTurnColor = new Color(0.26f, 0.64f, 0.28f, 0.96f);
        private static readonly Color DisabledTextColor = new Color(0.75f, 0.75f, 0.75f, 0.95f);
        private static readonly Color EnabledTextColor = new Color(1f, 1f, 1f, 0.98f);

        private void Awake()
        {
            ResolveNavigator();
        }

        private void OnEnable()
        {
            ResolveNavigator();

            if (navigator == null)
            {
                Debug.LogError($"{nameof(CombatPrototypeScreen)} requires a {nameof(ScreenNavigator)}.", this);
                return;
            }

            if (backToMainMenuButton != null)
            {
                backToMainMenuButton.onClick.AddListener(navigator.GoToMainMenu);
            }

            SubscribeToCombatEventsIfPossible();
        }

        private void OnDisable()
        {
            if (navigator == null)
            {
                return;
            }

            if (backToMainMenuButton != null)
            {
                backToMainMenuButton.onClick.RemoveListener(navigator.GoToMainMenu);
            }

            if (_basicAttackButton != null)
            {
                _basicAttackButton.onClick.RemoveListener(OnBasicAttackPressed);
            }

            if (_nextFloorButton != null)
            {
                _nextFloorButton.onClick.RemoveListener(OnNextFloorPressed);
            }

            UnsubscribeFromCombatEvents();

            if (_enemyTurnRoutine != null)
            {
                StopCoroutine(_enemyTurnRoutine);
                _enemyTurnRoutine = null;
            }
        }

        private void Start()
        {
            EnsureRuntimeUi();
            PrepareLegacySceneVisuals();
            SubscribeToCombatEventsIfPossible();

            if (_isCombatInitialized)
            {
                return;
            }

            InitializeCombat();
        }

        private void PrepareLegacySceneVisuals()
        {
            var content = transform.Find("Content");

            if (backToMainMenuButton != null)
            {
                var backTransform = backToMainMenuButton.transform as RectTransform;
                if (backTransform != null)
                {
                    backTransform.SetParent(transform, false);
                    backTransform.anchorMin = new Vector2(1f, 0f);
                    backTransform.anchorMax = new Vector2(1f, 0f);
                    backTransform.pivot = new Vector2(1f, 0f);
                    backTransform.anchoredPosition = new Vector2(-20f, 18f);
                    backTransform.sizeDelta = new Vector2(230f, 54f);
                    backTransform.SetAsLastSibling();
                }

                var graphic = backToMainMenuButton.targetGraphic as Image;
                if (graphic != null)
                {
                    graphic.color = new Color(0.16f, 0.25f, 0.34f, 0.88f);
                }
            }

            if (content == null)
            {
                return;
            }

            for (var index = 0; index < content.childCount; index++)
            {
                var child = content.GetChild(index);

                if (backToMainMenuButton != null && child == backToMainMenuButton.transform)
                {
                    continue;
                }

                child.gameObject.SetActive(false);
            }
        }

        private void ResolveNavigator()
        {
            if (navigator == null)
            {
                navigator = GetComponent<ScreenNavigator>();
            }
        }

        private void SubscribeToCombatEventsIfPossible()
        {
            if (_subscribedToEvents)
            {
                return;
            }

            if (_eventBus == null && !ServiceRegistry.TryResolve<IEventBus>(out _eventBus))
            {
                _eventBus = new EventBus();
            }

            _eventBus.Subscribe<CombatStartedEvent>(OnCombatStarted);
            _eventBus.Subscribe<CombatStateChangedEvent>(OnCombatStateChanged);
            _eventBus.Subscribe<TurnStartedEvent>(OnTurnStarted);
            _eventBus.Subscribe<DamageResolvedEvent>(OnDamageResolved);
            _eventBus.Subscribe<CombatEndedEvent>(OnCombatEnded);
            _eventBus.Subscribe<CombatVictoryEvent>(OnCombatVictory);
            _eventBus.Subscribe<CombatDefeatEvent>(OnCombatDefeat);
            _subscribedToEvents = true;
        }

        private void UnsubscribeFromCombatEvents()
        {
            if (!_subscribedToEvents || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<CombatStartedEvent>(OnCombatStarted);
            _eventBus.Unsubscribe<CombatStateChangedEvent>(OnCombatStateChanged);
            _eventBus.Unsubscribe<TurnStartedEvent>(OnTurnStarted);
            _eventBus.Unsubscribe<DamageResolvedEvent>(OnDamageResolved);
            _eventBus.Unsubscribe<CombatEndedEvent>(OnCombatEnded);
            _eventBus.Unsubscribe<CombatVictoryEvent>(OnCombatVictory);
            _eventBus.Unsubscribe<CombatDefeatEvent>(OnCombatDefeat);
            _subscribedToEvents = false;
        }

        private void InitializeCombat()
        {
            if (!_isUiReady)
            {
                return;
            }

            ServiceRegistry.TryResolve<IDungeonRunService>(out _dungeonRunService);
            ConfigureRunThemeFromTestDataIfNeeded();

            _formation = CreateDefaultFormation();
            BindFormationToSlots();
            SetResultVisible(false, string.Empty);
            _statusText.text = BuildFloorStatusText();

            try
            {
                if (_dungeonRunService != null && _dungeonRunService.HasActiveRun)
                {
                    _combatController = _dungeonRunService.CurrentCombatController
                        ?? _dungeonRunService.StartCurrentFloorCombat(_formation);
                }
                else
                {
                    _combatController = new CombatController(_formation, _eventBus);
                    _combatController.StartCombat();
                }

                _isCombatInitialized = true;
                RefreshAllVisuals();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                _statusText.text = "Failed to start combat.";
            }
        }

        private void ConfigureRunThemeFromTestDataIfNeeded()
        {
            if (_dungeonRunService is not DungeonRunService concreteService
                || !_dungeonRunService.HasActiveRun
                || concreteService.CurrentThemeDefinition != null
                || testData == null
                || testData.EnemyDefinitions.Length == 0
                || testData.EnemyDefinitions[0] == null
                || testData.EnemyDefinitions[0].Theme == null)
            {
                return;
            }

            concreteService.CurrentThemeDefinition = testData.EnemyDefinitions[0].Theme;
            if (_dungeonRunService.ActiveRun.CurrentFloorInfo == null
                || _dungeonRunService.ActiveRun.CurrentFloorInfo.Encounter == null)
            {
                _dungeonRunService.GenerateCurrentFloor();
            }
        }

        private CombatFormationState CreateDefaultFormation()
        {
            var formation = new CombatFormationState();

            if (_dungeonRunService != null && _dungeonRunService.HasActiveRun)
            {
                AddRunPartyOrFallbackHeroes(formation);
                AddRunEncounterOrFallbackEnemies(formation);
                return formation;
            }

            if (testData != null && testData.HeroDefinitions.Length >= 4 && testData.EnemyDefinitions.Length >= 1)
            {
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[0], 1));
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[1], 2));
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[2], 3));
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[3], 4));

                for (var rank = 1; rank <= CombatRank.MaxCombatantsPerSide; rank++)
                {
                    formation.AddCombatant(CombatantStateFactory.CreateEnemy(testData.EnemyDefinitions[0], rank));
                }
            }
            else
            {
                formation.AddCombatant(CreateCombatant("hero_front", "Knight", CombatSide.Player, 1, 28, 8, 4, 9));
                formation.AddCombatant(CreateCombatant("hero_mid", "Ranger", CombatSide.Player, 2, 22, 10, 2, 12));
                formation.AddCombatant(CreateCombatant("hero_back", "Mage", CombatSide.Player, 3, 18, 11, 1, 11));
                formation.AddCombatant(CreateCombatant("hero_support", "Cleric", CombatSide.Player, 4, 24, 6, 3, 8));

                formation.AddCombatant(CreateCombatant("enemy_brute", "Brute", CombatSide.Enemy, 1, 30, 7, 3, 7));
                formation.AddCombatant(CreateCombatant("enemy_raider", "Raider", CombatSide.Enemy, 2, 20, 8, 2, 10));
                formation.AddCombatant(CreateCombatant("enemy_shaman", "Shaman", CombatSide.Enemy, 3, 16, 9, 1, 11));
                formation.AddCombatant(CreateCombatant("enemy_guard", "Guard", CombatSide.Enemy, 4, 25, 6, 4, 6));
            }

            return formation;
        }

        private void AddRunPartyOrFallbackHeroes(CombatFormationState formation)
        {
            var party = _dungeonRunService.ActiveRun.Party;
            if (party != null && party.Count > 0)
            {
                for (var index = 0; index < party.Count; index++)
                {
                    formation.AddCombatant(party[index]);
                }

                return;
            }

            if (testData != null && testData.HeroDefinitions.Length >= 4)
            {
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[0], 1));
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[1], 2));
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[2], 3));
                formation.AddCombatant(CombatantStateFactory.CreateHero(testData.HeroDefinitions[3], 4));
                _dungeonRunService.ActiveRun.Party = new List<CombatantState>
                {
                    FindCombatantInFormation(formation, CombatSide.Player, 1),
                    FindCombatantInFormation(formation, CombatSide.Player, 2),
                    FindCombatantInFormation(formation, CombatSide.Player, 3),
                    FindCombatantInFormation(formation, CombatSide.Player, 4)
                };
                return;
            }

            formation.AddCombatant(CreateCombatant("hero_front", "Knight", CombatSide.Player, 1, 28, 8, 4, 9));
            formation.AddCombatant(CreateCombatant("hero_mid", "Ranger", CombatSide.Player, 2, 22, 10, 2, 12));
            formation.AddCombatant(CreateCombatant("hero_back", "Mage", CombatSide.Player, 3, 18, 11, 1, 11));
            formation.AddCombatant(CreateCombatant("hero_support", "Cleric", CombatSide.Player, 4, 24, 6, 3, 8));
            _dungeonRunService.ActiveRun.Party = new List<CombatantState>
            {
                FindCombatantInFormation(formation, CombatSide.Player, 1),
                FindCombatantInFormation(formation, CombatSide.Player, 2),
                FindCombatantInFormation(formation, CombatSide.Player, 3),
                FindCombatantInFormation(formation, CombatSide.Player, 4)
            };
        }

        private void AddRunEncounterOrFallbackEnemies(CombatFormationState formation)
        {
            var boss = ResolveBossDefinitionForCurrentFloor();
            if (boss != null)
            {
                formation.AddCombatant(CombatantStateFactory.CreateBoss(boss, CombatRank.Front));
                return;
            }

            var enemy = ResolveEnemyDefinitionForCurrentFloor();
            if (enemy != null)
            {
                AddEnemyDefinitionToFormation(formation, enemy);
                return;
            }

            formation.AddCombatant(CreateCombatant("enemy_brute", "Brute", CombatSide.Enemy, 1, 30, 7, 3, 7));
            formation.AddCombatant(CreateCombatant("enemy_raider", "Raider", CombatSide.Enemy, 2, 20, 8, 2, 10));
            formation.AddCombatant(CreateCombatant("enemy_shaman", "Shaman", CombatSide.Enemy, 3, 16, 9, 1, 11));
            formation.AddCombatant(CreateCombatant("enemy_guard", "Guard", CombatSide.Enemy, 4, 25, 6, 4, 6));
        }

        private BossDefinition ResolveBossDefinitionForCurrentFloor()
        {
            if (testData == null || testData.BossDefinitions.Length == 0)
            {
                return null;
            }

            var encounter = _dungeonRunService.ActiveRun.CurrentFloorInfo?.Encounter;
            if (encounter == null || encounter.DefinitionType != nameof(BossDefinition))
            {
                return null;
            }

            return testData.GetBoss(encounter.DefinitionId);
        }

        private EnemyDefinition ResolveEnemyDefinitionForCurrentFloor()
        {
            if (testData == null || testData.EnemyDefinitions.Length == 0)
            {
                return null;
            }

            var encounter = _dungeonRunService.ActiveRun.CurrentFloorInfo?.Encounter;
            if (encounter != null)
            {
                var resolved = testData.GetEnemy(encounter.DefinitionId);
                if (resolved != null)
                {
                    return resolved;
                }
            }

            return testData.EnemyDefinitions[0];
        }

        private string BuildFloorStatusText()
        {
            if (_dungeonRunService == null || !_dungeonRunService.HasActiveRun)
            {
                return "Tap Basic Attack, then tap a valid enemy target.";
            }

            var floor = _dungeonRunService.ActiveRun.CurrentFloorInfo;
            if (floor == null)
            {
                return "Tap Basic Attack, then tap a valid enemy target.";
            }

            var label = floor.PrimaryType == DungeonCrawler.Dungeon.FloorType.Boss
                ? $"Floor {floor.FloorNumber} - Boss Floor"
                : $"Floor {floor.FloorNumber}";

            if (floor.Encounter != null)
            {
                label += $" - {floor.Encounter.DisplayName}";
            }

            return label;
        }

        private static void AddEnemyDefinitionToFormation(CombatFormationState formation, EnemyDefinition enemy)
        {
            var occupiedRanks = enemy.OccupiedRanks;
            if (occupiedRanks != null && occupiedRanks.Length > 0)
            {
                for (var index = 0; index < occupiedRanks.Length; index++)
                {
                    var rank = occupiedRanks[index];
                    if (rank >= CombatRank.Front && rank <= CombatRank.Back && !formation.ContainsRank(CombatSide.Enemy, rank))
                    {
                        formation.AddCombatant(CombatantStateFactory.CreateEnemy(enemy, rank));
                    }
                }
            }

            if (formation.CountSide(CombatSide.Enemy) == 0)
            {
                formation.AddCombatant(CombatantStateFactory.CreateEnemy(enemy, 1));
            }
        }

        private static CombatantState FindCombatantInFormation(
            CombatFormationState formation,
            CombatSide side,
            int rank)
        {
            var combatants = formation.Combatants;
            for (var index = 0; index < combatants.Count; index++)
            {
                var combatant = combatants[index];
                if (combatant.Side == side && combatant.Rank == rank)
                {
                    return combatant;
                }
            }

            return null;
        }

        private static CombatantState CreateCombatant(
            string id,
            string displayName,
            CombatSide side,
            int rank,
            int maxHp,
            int attack,
            int defense,
            int speed)
        {
            return new CombatantState(id, displayName, side, rank, CreateStats(maxHp, attack, defense, speed));
        }

        private static CombatStats CreateStats(int maxHp, int attack, int defense, int speed)
        {
            object boxedStats = new CombatStats();
            SetPrivateField(boxedStats, "maxHp", maxHp);
            SetPrivateField(boxedStats, "attack", attack);
            SetPrivateField(boxedStats, "defense", defense);
            SetPrivateField(boxedStats, "speed", speed);
            return (CombatStats)boxedStats;
        }

        private static void SetPrivateField(object target, string fieldName, int value)
        {
            var field = typeof(CombatStats).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                throw new MissingFieldException(typeof(CombatStats).Name, fieldName);
            }

            field.SetValue(target, value);
        }

        private void EnsureRuntimeUi()
        {
            if (_isUiReady)
            {
                return;
            }

#if UNITY_EDITOR
            AutoLoadPortraitsInEditor();
#endif

            _runtimeRoot = CreateRectTransform("Combat Runtime Root", transform as RectTransform);
            StretchToFill(_runtimeRoot);

            EnsureCanvasScaler();

            var rootLayout = _runtimeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(6, 6, 6, 6);
            rootLayout.spacing = 6;
            rootLayout.childControlHeight = true;
            rootLayout.childControlWidth = true;
            rootLayout.childForceExpandHeight = true;
            rootLayout.childForceExpandWidth = true;

            CreateStatusHeader();

            var battlefield = CreateRectTransform("Battlefield", _runtimeRoot);
            var battlefieldLayout = battlefield.gameObject.AddComponent<HorizontalLayoutGroup>();
            battlefieldLayout.spacing = 4;
            battlefieldLayout.childAlignment = TextAnchor.UpperCenter;
            battlefieldLayout.childControlHeight = true;
            battlefieldLayout.childControlWidth = true;
            battlefieldLayout.childForceExpandHeight = true;
            battlefieldLayout.childForceExpandWidth = true;

            var battlefieldSize = battlefield.gameObject.AddComponent<LayoutElement>();
            battlefieldSize.flexibleHeight = 1;

            CreateSideSection("Party", CombatSide.Player, _playerSlots, battlefield);
            CreateSideSection("Enemies", CombatSide.Enemy, _enemySlots, battlefield);

            CreateBottomActionBar();
            CreateResultOverlay();

            _isUiReady = true;
        }

        private void CreateSideSection(string title, CombatSide side, List<CombatantSlotView> target, Transform parent)
        {
            var section = CreatePanel(title + " Section", parent, new Color(0.12f, 0.13f, 0.17f, 0.82f));
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(4, 4, 4, 4);
            layout.spacing = 4;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = true;

            var sectionSize = section.gameObject.AddComponent<LayoutElement>();
            sectionSize.flexibleHeight = 1;
            sectionSize.flexibleWidth = 1;

            CreateLabel(title, section, 16, TextAnchor.MiddleLeft);

            var gridRoot = CreateRectTransform(title + " Grid", section);
            var gridLayout = gridRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            gridLayout.spacing = 6;
            gridLayout.childControlHeight = true;
            gridLayout.childControlWidth = true;
            gridLayout.childForceExpandHeight = true;
            gridLayout.childForceExpandWidth = true;

            var gridSize = gridRoot.gameObject.AddComponent<LayoutElement>();
            gridSize.flexibleHeight = 1;
            gridSize.flexibleWidth = 1;

            for (var rank = 1; rank <= CombatRank.MaxCombatantsPerSide; rank++)
            {
                var slotRoot = CreatePanel($"{side} Slot {rank}", gridRoot, EmptySlotColor);
                var layoutElement = slotRoot.gameObject.AddComponent<LayoutElement>();
                layoutElement.flexibleWidth = 1;
                layoutElement.flexibleHeight = 1;
                layoutElement.minHeight = 100;

                var button = slotRoot.gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.ColorTint;
                button.targetGraphic = slotRoot.GetComponent<Image>();

                var slotLayout = slotRoot.gameObject.AddComponent<VerticalLayoutGroup>();
                slotLayout.padding = new RectOffset(2, 2, 1, 2);
                slotLayout.spacing = 1;
                slotLayout.childAlignment = TextAnchor.UpperCenter;
                slotLayout.childControlHeight = true;
                slotLayout.childControlWidth = true;
                slotLayout.childForceExpandHeight = true;
                slotLayout.childForceExpandWidth = true;

                var portraitImage = CreatePortrait(slotLayout.transform);

                var nameText = CreateLabel($"R{rank}", slotLayout.transform, 11, TextAnchor.MiddleCenter);
                var hpText = CreateLabel("HP --/--", slotLayout.transform, 10, TextAnchor.MiddleCenter);
                var statusText = CreateLabel(side == CombatSide.Player ? "P" : "E", slotLayout.transform, 9, TextAnchor.MiddleCenter);
                statusText.color = new Color(1f, 1f, 1f, 0.9f);

                var slot = new CombatantSlotView(side, rank, slotRoot, button, portraitImage, nameText, hpText, statusText);
                button.onClick.AddListener(() => OnCombatantSlotClicked(slot));
                target.Add(slot);
            }
        }

        private static Image CreatePortrait(Transform parent)
        {
            var rect = CreateRectTransform("Portrait", parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.9f, 0.9f, 0.9f, 0.85f);
            image.preserveAspect = true;
            image.maskable = false;

            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1;
            layout.flexibleHeight = 1;
            layout.preferredHeight = 80;
            layout.minHeight = 60;

            rect.localScale = new Vector3(1.12f, 1.12f, 1f);

            return image;
        }

        private void EnsureCanvasScaler()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(414f, 896f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        private void CreateStatusHeader()
        {
            var section = CreatePanel("Status Header", _runtimeRoot, new Color(0.08f, 0.09f, 0.12f, 0.9f));
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 6;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            var sectionSize = section.gameObject.AddComponent<LayoutElement>();
            sectionSize.preferredHeight = 120;

            _turnText = CreateLabel("Turn: --", section, 30, TextAnchor.MiddleCenter);
            _statusText = CreateLabel("Waiting combat start...", section, 22, TextAnchor.MiddleCenter);
        }

        private void CreateBottomActionBar()
        {
            var section = CreatePanel("Bottom Action Bar", _runtimeRoot, new Color(0.08f, 0.09f, 0.12f, 0.9f));
            var layout = section.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 12;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var sectionSize = section.gameObject.AddComponent<LayoutElement>();
            sectionSize.preferredHeight = 96;

            _basicAttackButton = CreateTextButton("Basic Attack", section, OnBasicAttackPressed);
            _basicAttackButton.interactable = false;
        }

        private void CreateResultOverlay()
        {
            var overlay = CreatePanel("Result Overlay", _runtimeRoot, new Color(0f, 0f, 0f, 0.82f));
            StretchToFill(overlay);
            overlay.SetAsLastSibling();
            var overlayLayoutElement = overlay.gameObject.AddComponent<LayoutElement>();
            overlayLayoutElement.ignoreLayout = true;

            var layout = overlay.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;

            _resultText = CreateLabel(string.Empty, overlay, 48, TextAnchor.MiddleCenter);
            _resultText.resizeTextForBestFit = true;
            _resultText.resizeTextMinSize = 22;
            _resultText.resizeTextMaxSize = 52;
            _nextFloorButton = CreateTextButton("Next Floor", overlay, OnNextFloorPressed);
            overlay.gameObject.SetActive(false);
        }

        private void BindFormationToSlots()
        {
            _slotsByCombatant.Clear();

            for (var index = 0; index < _playerSlots.Count; index++)
            {
                BindSlot(_playerSlots[index], FindCombatant(CombatSide.Player, _playerSlots[index].Rank));
            }

            for (var index = 0; index < _enemySlots.Count; index++)
            {
                BindSlot(_enemySlots[index], FindCombatant(CombatSide.Enemy, _enemySlots[index].Rank));
            }
        }

        private void BindSlot(CombatantSlotView slot, CombatantState combatant)
        {
            slot.SetCombatant(combatant);
            slot.SetPortrait(GetPortraitForSlot(slot.Side, slot.Rank));
            if (combatant != null)
            {
                _slotsByCombatant[combatant] = slot;
            }
        }

        private Sprite GetPortraitForSlot(CombatSide side, int rank)
        {
            var source = side == CombatSide.Player ? playerPortraits : enemyPortraits;
            var index = rank - 1;

            if (source == null || index < 0 || index >= source.Length)
            {
                return null;
            }

            return source[index];
        }

#if UNITY_EDITOR
        private void AutoLoadPortraitsInEditor()
        {
            if (playerPortraits == null || playerPortraits.Length < CombatRank.MaxCombatantsPerSide)
            {
                playerPortraits = new Sprite[CombatRank.MaxCombatantsPerSide];
            }

            if (enemyPortraits == null || enemyPortraits.Length < CombatRank.MaxCombatantsPerSide)
            {
                enemyPortraits = new Sprite[CombatRank.MaxCombatantsPerSide];
            }

            for (var index = 0; index < CombatRank.MaxCombatantsPerSide; index++)
            {
                if (playerPortraits[index] == null)
                {
                    var heroPath = $"Assets/Game/Art/Heroes/hero_{index + 1}.png";
                    playerPortraits[index] = LoadSpriteFromPath(heroPath);
                }

                if (enemyPortraits[index] == null)
                {
                    var enemyPath = $"Assets/Game/Art/Enemies/enemy_{index + 1}.png";
                    enemyPortraits[index] = LoadSpriteFromPath(enemyPath);
                }
            }
        }

        private static Sprite LoadSpriteFromPath(string path)
        {
            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }

            var sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
            for (var index = 0; index < sprites.Length; index++)
            {
                if (sprites[index] is Sprite loadedSprite)
                {
                    return loadedSprite;
                }
            }

            return null;
        }
#endif

        private CombatantState FindCombatant(CombatSide side, int rank)
        {
            var combatants = _formation.Combatants;
            for (var index = 0; index < combatants.Count; index++)
            {
                var combatant = combatants[index];
                if (combatant.Side == side && combatant.Rank == rank)
                {
                    return combatant;
                }
            }

            return null;
        }

        private void OnBasicAttackPressed()
        {
            if (_combatController == null || _combatController.State != CombatState.PlayerTurn)
            {
                return;
            }

            _awaitingTargetSelection = true;
            _statusText.text = "Select an enemy target.";
            RefreshTargetSelectionVisuals();
        }

        private void OnCombatantSlotClicked(CombatantSlotView slot)
        {
            if (!_awaitingTargetSelection || _combatController == null || slot.Combatant == null)
            {
                return;
            }

            if (!CombatPrototypeTurnSelectionUtility.IsValidBasicAttackTarget(_combatController.CurrentCombatant, slot.Combatant))
            {
                _statusText.text = "Invalid target for a basic attack.";
                return;
            }

            _awaitingTargetSelection = false;

            try
            {
                var result = _combatController.ExecuteBasicAttack(slot.Combatant);
                _statusText.text = $"{result.Attacker.DisplayName} dealt {result.Damage} to {result.Target.DisplayName}.";
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                _statusText.text = "Attack failed.";
            }

            RefreshTargetSelectionVisuals();
        }

        private void OnCombatStarted(CombatStartedEvent gameEvent)
        {
            RefreshAllVisuals();
        }

        private void OnCombatStateChanged(CombatStateChangedEvent gameEvent)
        {
            RefreshAllVisuals();
        }

        private void OnTurnStarted(TurnStartedEvent gameEvent)
        {
            RefreshAllVisuals();

            if (gameEvent.State == CombatState.EnemyTurn)
            {
                QueueEnemyTurnAttack();
            }
            else if (_enemyTurnRoutine != null)
            {
                StopCoroutine(_enemyTurnRoutine);
                _enemyTurnRoutine = null;
            }
        }

        private void QueueEnemyTurnAttack()
        {
            if (_enemyTurnRoutine != null)
            {
                return;
            }

            _enemyTurnRoutine = StartCoroutine(ExecuteEnemyTurnDelayed());
        }

        private IEnumerator ExecuteEnemyTurnDelayed()
        {
            yield return new WaitForSecondsRealtime(EnemyTurnDelaySeconds);
            _enemyTurnRoutine = null;

            if (_combatController == null || _combatController.State != CombatState.EnemyTurn)
            {
                yield break;
            }

            var playerTargets = CombatPrototypeTurnSelectionUtility.GetLivingTargets(_formation.Combatants, CombatSide.Player);
            var target = CombatPrototypeTurnSelectionUtility.ChooseRandomTarget(playerTargets, _enemyTargetRandom);

            if (target == null)
            {
                yield break;
            }

            try
            {
                var result = _combatController.ExecuteBasicAttack(target);
                _statusText.text = $"{result.Attacker.DisplayName} attacked {result.Target.DisplayName} for {result.Damage}.";
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                _statusText.text = "Enemy turn failed.";
            }
        }

        private void OnDamageResolved(DamageResolvedEvent gameEvent)
        {
            RefreshAllVisuals();
        }

        private void OnCombatVictory(CombatVictoryEvent gameEvent)
        {
            _statusText.text = "Battle won.";
        }

        private void OnCombatDefeat(CombatDefeatEvent gameEvent)
        {
            _statusText.text = "Party was defeated.";
        }

        private void OnCombatEnded(CombatEndedEvent gameEvent)
        {
            _awaitingTargetSelection = false;
            RefreshAllVisuals();
            if (gameEvent.ResultState == CombatState.Victory)
            {
                var canAdvance = _dungeonRunService != null
                    && _dungeonRunService.HasActiveRun
                    && _dungeonRunService.ActiveRun.CanAdvanceFloor
                    && _dungeonRunService.ActiveRun.LastResolvedReward != null;
                SetResultVisible(true, BuildRewardSummary(), canAdvance);
                return;
            }

            SetResultVisible(true, "Run Failed");
        }

        private string BuildRewardSummary()
        {
            if (_dungeonRunService == null
                || !_dungeonRunService.HasActiveRun
                || _dungeonRunService.ActiveRun.LastResolvedReward == null)
            {
                return "Rewards\nPending...";
            }

            var reward = _dungeonRunService.ActiveRun.LastResolvedReward;
            var lines = new List<string>
            {
                reward.IsBossReward ? "Boss Rewards" : "Rewards",
                $"Gold: {reward.SoftCurrency}"
            };

            for (var index = 0; index < reward.ItemRewards.Count; index++)
            {
                var item = reward.ItemRewards[index];
                lines.Add($"Item: {item.DisplayName} x{item.Quantity}");
            }

            AppendFloorResolutionLines(lines);

            return string.Join("\n", lines);
        }

        private void AppendFloorResolutionLines(List<string> lines)
        {
            var floor = _dungeonRunService.ActiveRun.CurrentFloorInfo;
            if (floor == null)
            {
                return;
            }

            if (floor.HasRestingSite)
            {
                lines.Add("Resting Site Unlocked");
            }

            if (floor.IsThemeTransition)
            {
                lines.Add("Theme Transition Ready");
            }
        }

        private void RefreshAllVisuals()
        {
            RefreshSlotVisuals();
            RefreshTurnVisual();
            RefreshTargetSelectionVisuals();
        }

        private void RefreshSlotVisuals()
        {
            RefreshSlotCollection(_playerSlots);
            RefreshSlotCollection(_enemySlots);
        }

        private void RefreshSlotCollection(List<CombatantSlotView> slots)
        {
            for (var index = 0; index < slots.Count; index++)
            {
                slots[index].Refresh();
            }
        }

        private void RefreshTurnVisual()
        {
            if (_combatController == null)
            {
                _turnText.text = "Turn: --";
                return;
            }

            var active = _combatController.CurrentCombatant;
            _turnText.text = active == null
                ? $"Turn: {_combatController.State}"
                : $"Turn: {active.DisplayName} ({_combatController.State})";

            foreach (var pair in _slotsByCombatant)
            {
                pair.Value.SetActiveTurn(active != null && ReferenceEquals(pair.Key, active));
            }
        }

        private void RefreshTargetSelectionVisuals()
        {
            var playerCanAct = _combatController != null
                && _combatController.State == CombatState.PlayerTurn
                && _combatController.CurrentCombatant != null
                && _combatController.CurrentCombatant.IsAlive;

            var playerTargets = _combatController == null
                ? new List<CombatantState>()
                : CombatPrototypeTurnSelectionUtility.GetValidBasicAttackTargets(_combatController.CurrentCombatant, _formation.Combatants);

            for (var index = 0; index < _enemySlots.Count; index++)
            {
                var slot = _enemySlots[index];
                var selectable = _awaitingTargetSelection && playerCanAct && slot.Combatant != null && playerTargets.Contains(slot.Combatant);
                slot.SetSelectable(selectable);
                slot.SetHighlighted(selectable);
            }

            for (var index = 0; index < _playerSlots.Count; index++)
            {
                _playerSlots[index].SetSelectable(false);
                _playerSlots[index].SetHighlighted(false);
            }

            _basicAttackButton.interactable = playerCanAct && !_awaitingTargetSelection;
        }

        private void SetResultVisible(bool visible, string text, bool showNextFloor = false)
        {
            if (_resultText == null)
            {
                return;
            }

            _resultText.transform.parent.gameObject.SetActive(visible);
            _resultText.text = text;

            if (_nextFloorButton != null)
            {
                _nextFloorButton.gameObject.SetActive(visible && showNextFloor);
                _nextFloorButton.interactable = visible && showNextFloor;
            }
        }

        private void OnNextFloorPressed()
        {
            if (_dungeonRunService == null || !_dungeonRunService.HasActiveRun)
            {
                return;
            }

            try
            {
                _dungeonRunService.AdvanceFloor();
                _combatController = null;
                _formation = null;
                _isCombatInitialized = false;
                _awaitingTargetSelection = false;
                SetResultVisible(false, string.Empty);
                InitializeCombat();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                _statusText.text = "Failed to advance floor.";
            }
        }

        private static RectTransform CreateRectTransform(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color backgroundColor)
        {
            var panel = CreateRectTransform(name, parent);
            var image = panel.gameObject.AddComponent<Image>();
            image.color = backgroundColor;
            return panel;
        }

        private static Text CreateLabel(string text, Transform parent, int size, TextAnchor anchor)
        {
            var rectTransform = CreateRectTransform("Label", parent);
            var label = rectTransform.gameObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = text;
            label.fontSize = size;
            label.alignment = anchor;
            label.color = EnabledTextColor;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;

            var layout = rectTransform.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = size + 8;
            layout.flexibleWidth = 1;
            return label;
        }

        private static Button CreateTextButton(string label, Transform parent, Action callback)
        {
            var buttonRoot = CreatePanel("Button", parent, new Color(0.18f, 0.42f, 0.64f, 1f));
            var button = buttonRoot.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = buttonRoot.GetComponent<Image>();

            var layout = buttonRoot.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = 300;
            layout.preferredHeight = 72;

            var text = CreateLabel(label, buttonRoot, 24, TextAnchor.MiddleCenter);
            text.raycastTarget = false;

            button.onClick.AddListener(() => callback?.Invoke());
            return button;
        }

        private static void StretchToFill(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private sealed class CombatantSlotView
        {
            private readonly Image _background;
            private readonly Image _portrait;
            private readonly Text _name;
            private readonly Text _hp;
            private readonly Text _status;
            private readonly Button _button;

            public CombatantSlotView(
                CombatSide side,
                int rank,
                RectTransform root,
                Button button,
                Image portrait,
                Text name,
                Text hp,
                Text status)
            {
                Side = side;
                Rank = rank;
                Root = root;
                _button = button;
                _background = root.GetComponent<Image>();
                _portrait = portrait;
                _name = name;
                _hp = hp;
                _status = status;
            }

            public CombatSide Side { get; }

            public int Rank { get; }

            public RectTransform Root { get; }

            public CombatantState Combatant { get; private set; }

            public bool IsSelectable { get; private set; }

            public bool IsHighlighted { get; private set; }

            public bool IsActiveTurn { get; private set; }

            public Sprite Portrait { get; private set; }

            public void SetCombatant(CombatantState combatant)
            {
                Combatant = combatant;
                Refresh();
            }

            public void SetPortrait(Sprite portrait)
            {
                Portrait = portrait;
                Refresh();
            }

            public void SetSelectable(bool selectable)
            {
                IsSelectable = selectable;
                _button.interactable = selectable;
                Refresh();
            }

            public void SetHighlighted(bool highlighted)
            {
                IsHighlighted = highlighted;
                Refresh();
            }

            public void SetActiveTurn(bool active)
            {
                IsActiveTurn = active;
                Refresh();
            }

            public void Refresh()
            {
                if (Combatant == null)
                {
                    _name.text = $"Rank {Rank}: Empty";
                    _hp.text = "HP --/--";
                    _status.text = "No combatant";
                    _status.color = DisabledTextColor;
                    _background.color = EmptySlotColor;
                    _button.interactable = false;
                    _portrait.sprite = null;
                    _portrait.color = new Color(1f, 1f, 1f, 0f);
                    return;
                }

                _name.text = $"R{Combatant.Rank} {Combatant.DisplayName}";
                _hp.text = $"HP {Combatant.CurrentHp}/{Combatant.MaxHp}";
                _status.text = Combatant.IsAlive ? "Alive" : "Defeated";
                _status.color = Combatant.IsAlive ? EnabledTextColor : DisabledTextColor;

                _portrait.sprite = Portrait;
                _portrait.color = Portrait == null
                    ? new Color(1f, 1f, 1f, 0.15f)
                    : new Color(1f, 1f, 1f, Combatant.IsAlive ? 1f : 0.55f);

                var baseColor = Side == CombatSide.Player ? PlayerSlotColor : EnemySlotColor;
                if (IsActiveTurn)
                {
                    _background.color = ActiveTurnColor;
                }
                else if (IsHighlighted)
                {
                    _background.color = HighlightColor;
                }
                else
                {
                    _background.color = baseColor;
                }

                if (!Combatant.IsAlive)
                {
                    _background.color = Color.Lerp(_background.color, Color.black, 0.45f);
                    _button.interactable = false;
                }
                else
                {
                    _button.interactable = IsSelectable;
                }
            }
        }
    }
}
