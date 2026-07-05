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
        [SerializeField] private Sprite basicStrikeIcon;
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
        private Button _nextFloorButton;
        private readonly List<CombatantSlotView> _playerSlots = new List<CombatantSlotView>();
        private readonly List<CombatantSlotView> _enemySlots = new List<CombatantSlotView>();
        private readonly Dictionary<CombatantState, CombatantSlotView> _slotsByCombatant = new Dictionary<CombatantState, CombatantSlotView>();
        private Dictionary<string, HeroClassDefinition> _classDefsByHeroId;
        private SkillDefinition _activeSkillDefinition;
        private RectTransform _skillButtonsContainer;
        private readonly List<Button> _skillButtons = new List<Button>();

        private static readonly Color ClearSlotColor = new Color(1f, 1f, 1f, 0f);
        private static readonly Color PlayerHpColor = new Color(0.2f, 0.78f, 0.32f, 1f);
        private static readonly Color EnemyHpColor = new Color(0.86f, 0.18f, 0.18f, 1f);
        private static readonly Color ExpColor = new Color(0.3f, 0.56f, 0.95f, 1f);
        private static readonly Color BarBackgroundColor = new Color(0.04f, 0.05f, 0.06f, 0.82f);
        private static readonly Color HighlightColor = new Color(0.95f, 0.72f, 0.28f, 0.26f);
        private static readonly Color ActiveTurnColor = new Color(0.26f, 0.64f, 0.28f, 0.24f);
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

            ClearSkillButtons();

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
            BuildClassDefsDictionary();

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
                AddEnemyDefinitionToFormation(formation, enemy, ShouldLimitCurrentCombatToSingleEnemy());
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
                return "Choose a skill, then tap a valid target.";
            }

            var floor = _dungeonRunService.ActiveRun.CurrentFloorInfo;
            if (floor == null)
            {
                return "Choose a skill, then tap a valid target.";
            }

            var themeName = string.Empty;
            if (_dungeonRunService is DungeonRunService concrete && concrete.CurrentThemeDefinition != null)
            {
                themeName = $" [{concrete.CurrentThemeDefinition.DisplayName}]";
            }

            var label = floor.PrimaryType == DungeonCrawler.Dungeon.FloorType.Boss
                ? $"Floor {floor.FloorNumber} - Boss Floor"
                : $"Floor {floor.FloorNumber}";

            if (floor.Encounter != null)
            {
                label += $" - {floor.Encounter.DisplayName}";
            }

            label += themeName;

            return label;
        }

        private bool ShouldLimitCurrentCombatToSingleEnemy()
        {
            return _dungeonRunService != null
                && _dungeonRunService.HasActiveRun
                && ShouldLimitCombatToSingleEnemy(_dungeonRunService.ActiveRun.CurrentFloor);
        }

        public static bool ShouldLimitCombatToSingleEnemy(int floorNumber)
        {
            return floorNumber > 0 && floorNumber < 5;
        }

        private static void AddEnemyDefinitionToFormation(
            CombatFormationState formation,
            EnemyDefinition enemy,
            bool singleEnemyOnly = false)
        {
            if (singleEnemyOnly)
            {
                formation.AddCombatant(CombatantStateFactory.CreateEnemy(enemy, CombatRank.Front));
                return;
            }

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
            rootLayout.padding = new RectOffset(6, 6, 50, 6);
            rootLayout.spacing = 6;
            rootLayout.childControlHeight = true;
            rootLayout.childControlWidth = true;
            rootLayout.childForceExpandHeight = true;
            rootLayout.childForceExpandWidth = true;

            CreateStatusHeader();

            var battlefield = CreateRectTransform("Battlefield", _runtimeRoot);
            var battlefieldLayout = battlefield.gameObject.AddComponent<HorizontalLayoutGroup>();
            battlefieldLayout.spacing = 4;
            battlefieldLayout.childAlignment = TextAnchor.MiddleCenter;
            battlefieldLayout.childControlHeight = true;
            battlefieldLayout.childControlWidth = true;
            battlefieldLayout.childForceExpandHeight = true;
            battlefieldLayout.childForceExpandWidth = true;

            var battlefieldSize = battlefield.gameObject.AddComponent<LayoutElement>();
            battlefieldSize.flexibleHeight = 1;

            CreateAllCharacterSlots(battlefield);

            CreateBottomActionBar();
            CreateResultOverlay();

            _isUiReady = true;
        }

        private void CreateAllCharacterSlots(Transform parent)
        {
            for (var rank = 1; rank <= CombatRank.MaxCombatantsPerSide; rank++)
            {
                CreateSingleSlot($"Player Slot {rank}", CombatSide.Player, rank, _playerSlots, parent);
            }
            for (var rank = 1; rank <= CombatRank.MaxCombatantsPerSide; rank++)
            {
                CreateSingleSlot($"Enemy Slot {rank}", CombatSide.Enemy, rank, _enemySlots, parent);
            }
        }

        private void CreateSingleSlot(string name, CombatSide side, int rank, List<CombatantSlotView> target, Transform parent)
        {
            var slotRoot = CreateRectTransform(name, parent);
            var layoutElement = slotRoot.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1;
            layoutElement.flexibleHeight = 1;
            layoutElement.minHeight = 100;

            var slotImage = slotRoot.gameObject.AddComponent<Image>();
            slotImage.color = ClearSlotColor;

            var button = slotRoot.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = slotImage;

            var slotLayout = slotRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            slotLayout.padding = new RectOffset(2, 2, 1, 2);
            slotLayout.spacing = 3;
            slotLayout.childAlignment = TextAnchor.MiddleCenter;
            slotLayout.childControlHeight = true;
            slotLayout.childControlWidth = true;
            slotLayout.childForceExpandHeight = false;
            slotLayout.childForceExpandWidth = true;

            var hpFill = CreateBar("HP Bar", slotLayout.transform, side == CombatSide.Player ? PlayerHpColor : EnemyHpColor);
            var expFill = side == CombatSide.Player
                ? CreateBar("EXP Bar", slotLayout.transform, ExpColor)
                : null;
            var portraitImage = CreatePortrait(slotLayout.transform);

            var slot = new CombatantSlotView(
                side,
                rank,
                slotRoot,
                button,
                portraitImage,
                hpFill,
                expFill,
                GetExpProgressForCombatant);
            button.onClick.AddListener(() => OnCombatantSlotClicked(slot));
            target.Add(slot);
        }

        private static Image CreateBar(string name, Transform parent, Color fillColor)
        {
            var background = CreateRectTransform(name, parent);
            var backgroundImage = background.gameObject.AddComponent<Image>();
            backgroundImage.color = BarBackgroundColor;

            var layout = background.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 8;
            layout.minHeight = 8;
            layout.flexibleWidth = 1;

            var fill = CreateRectTransform("Fill", background);
            StretchToFill(fill);
            var fillImage = fill.gameObject.AddComponent<Image>();
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 1f;
            fillImage.raycastTarget = false;

            return fillImage;
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
            var section = CreateRectTransform("Status Header", _runtimeRoot);
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 6;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            var sectionSize = section.gameObject.AddComponent<LayoutElement>();
            sectionSize.preferredHeight = 100;

            _turnText = CreateLabel("Turn: --", section, 24, TextAnchor.MiddleCenter);
            _statusText = CreateLabel("Waiting combat start...", section, 18, TextAnchor.MiddleCenter);
        }

        private void CreateBottomActionBar()
        {
            var section = CreateRectTransform("Bottom Action Bar", _runtimeRoot);
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

            _skillButtonsContainer = CreateRectTransform("Skill Buttons", section);
            var skillLayout = _skillButtonsContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            skillLayout.spacing = 12;
            skillLayout.childAlignment = TextAnchor.MiddleCenter;
            skillLayout.childControlHeight = false;
            skillLayout.childControlWidth = false;
            skillLayout.childForceExpandHeight = false;
            skillLayout.childForceExpandWidth = false;

            var skillSize = _skillButtonsContainer.gameObject.AddComponent<LayoutElement>();
            skillSize.preferredHeight = 72;
            skillSize.flexibleWidth = 1;
        }

        private void CreateResultOverlay()
        {
            var overlay = CreateRectTransform("Result Overlay", _runtimeRoot);
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

        private float GetExpProgressForCombatant(CombatantState combatant)
        {
            var hero = FindRosterHero(combatant);
            if (hero == null || hero.IsMaxLevel)
            {
                return hero == null ? 0f : 1f;
            }

            return hero.XpToNextLevel <= 0
                ? 0f
                : Mathf.Clamp01((float)hero.CurrentXp / hero.XpToNextLevel);
        }

        private DungeonCrawler.Data.State.HeroState FindRosterHero(CombatantState combatant)
        {
            var roster = _dungeonRunService?.ActiveRun?.Roster;
            if (combatant == null || roster == null)
            {
                return null;
            }

            for (var i = 0; i < roster.Count; i++)
            {
                var hero = roster[i];
                if (hero != null
                    && hero.ClassId == combatant.DefinitionId
                    && hero.PartyRank == combatant.Rank)
                {
                    return hero;
                }
            }

            return null;
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

            if (basicStrikeIcon == null)
            {
                basicStrikeIcon = LoadSpriteFromPath("Assets/Game/Art/Skills/basic_strike.png");
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

            _activeSkillDefinition = null;
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

            if (_activeSkillDefinition != null)
            {
                var validation = _combatController.ValidateSkillTargetForCurrentCombatant(
                    _activeSkillDefinition, slot.Combatant);
                if (!validation.IsValid)
                {
                    _statusText.text = validation.ErrorMessage;
                    return;
                }

                _awaitingTargetSelection = false;
                var skill = _activeSkillDefinition;
                _activeSkillDefinition = null;

                try
                {
                    var result = _combatController.ExecuteSkill(skill, slot.Combatant);
                    _statusText.text = $"{result.Attacker.DisplayName} used {skill.DisplayName} on {result.Target.DisplayName} for {result.Damage}.";
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                    _statusText.text = "Skill execution failed.";
                }
            }
            else
            {
                if (!CombatPrototypeTurnSelectionUtility.IsValidBasicAttackTarget(
                    _combatController.CurrentCombatant, slot.Combatant))
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

            if (gameEvent.State == CombatState.PlayerTurn)
            {
                RebuildSkillButtonsForCurrentHero();
            }
            else
            {
                ClearSkillButtons();
            }

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
            _activeSkillDefinition = null;
            ClearSkillButtons();
            RefreshAllVisuals();
            if (gameEvent.ResultState == CombatState.Victory)
            {
                var canAdvance = _dungeonRunService != null
                    && _dungeonRunService.HasActiveRun
                    && _dungeonRunService.ActiveRun.CanAdvanceFloor
                    && _dungeonRunService.ActiveRun.LastResolvedReward != null;

                if (canAdvance && HasRestingSiteOnCurrentFloor())
                {
                    ShowRestingSiteTransition();
                    return;
                }

                SetResultVisible(true, BuildRewardSummary(), canAdvance);
                return;
            }

            SetResultVisible(true, "Run Failed");
        }

        private bool HasRestingSiteOnCurrentFloor()
        {
            if (_dungeonRunService?.ActiveRun?.CurrentFloorInfo == null)
            {
                return false;
            }

            return _dungeonRunService.ActiveRun.CurrentFloorInfo.HasRestingSite;
        }

        private void ShowRestingSiteTransition()
        {
            var floor = _dungeonRunService.ActiveRun.CurrentFloorInfo;
            var floorNum = floor?.FloorNumber ?? 0;
            _resultText.text = $"Floor {floorNum} Cleared!\n\nResting Site Ahead.\n\n{_dungeonRunService.ActiveRun.LastResolvedReward?.SoftCurrency ?? 0} Gold earned.";

            var resultParent = _resultText.transform.parent.gameObject;
            resultParent.SetActive(true);

            if (_nextFloorButton != null)
            {
                _nextFloorButton.gameObject.SetActive(true);
                _nextFloorButton.interactable = true;
                var text = _nextFloorButton.GetComponentInChildren<Text>();
                if (text != null) text.text = "Enter Rest Site";
                _nextFloorButton.onClick.RemoveListener(OnNextFloorPressed);
                _nextFloorButton.onClick.AddListener(OnEnterRestSitePressed);
            }
        }

        private void OnEnterRestSitePressed()
        {
            if (_nextFloorButton != null)
            {
                _nextFloorButton.onClick.RemoveListener(OnEnterRestSitePressed);
                _nextFloorButton.onClick.AddListener(OnNextFloorPressed);
            }

            if (navigator != null)
            {
                navigator.GoToRestSite();
            }
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

            List<CombatantState> validTargets;
            if (_activeSkillDefinition != null)
            {
                var skillTargets = _combatController != null
                    ? _combatController.GetValidTargetsForCurrentCombatant(_activeSkillDefinition)
                    : null;
                validTargets = skillTargets != null
                    ? new List<CombatantState>(skillTargets)
                    : new List<CombatantState>();
            }
            else
            {
                validTargets = _combatController == null
                    ? new List<CombatantState>()
                    : CombatPrototypeTurnSelectionUtility.GetValidBasicAttackTargets(
                        _combatController.CurrentCombatant, _formation.Combatants);
            }

            foreach (var pair in _slotsByCombatant)
            {
                var selectable = _awaitingTargetSelection
                    && playerCanAct
                    && pair.Key.IsAlive
                    && validTargets.Contains(pair.Key);
                pair.Value.SetSelectable(selectable);
                pair.Value.SetHighlighted(selectable);
            }

            for (var index = 0; index < _skillButtons.Count; index++)
            {
                _skillButtons[index].interactable = playerCanAct && !_awaitingTargetSelection;
            }
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
                _activeSkillDefinition = null;
                _classDefsByHeroId = null;
                ClearSkillButtons();
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

        private void BuildClassDefsDictionary()
        {
            if (testData == null || testData.HeroDefinitions == null || testData.HeroDefinitions.Length == 0)
            {
                return;
            }

            _classDefsByHeroId = new Dictionary<string, HeroClassDefinition>();
            var defs = testData.HeroDefinitions;
            for (var i = 0; i < defs.Length; i++)
            {
                var def = defs[i];
                if (def != null && !string.IsNullOrEmpty(def.Id) && !_classDefsByHeroId.ContainsKey(def.Id))
                {
                    _classDefsByHeroId[def.Id] = def;
                }
            }
        }

        private void RebuildSkillButtonsForCurrentHero()
        {
            ClearSkillButtons();

            if (_skillButtonsContainer == null
                || _combatController?.CurrentCombatant == null
                || _combatController.CurrentCombatant.Side != CombatSide.Player)
            {
                return;
            }

            var skills = GetSkillsForCombatant(_combatController.CurrentCombatant);
            if (skills == null || skills.Length == 0)
            {
                return;
            }

            for (var i = 0; i < skills.Length; i++)
            {
                var skill = skills[i];
                if (skill == null) continue;
                var button = CreateSkillButton(skill, _skillButtonsContainer);
                _skillButtons.Add(button);
            }
        }

        private void ClearSkillButtons()
        {
            _activeSkillDefinition = null;
            for (var i = 0; i < _skillButtons.Count; i++)
            {
                if (_skillButtons[i] != null)
                {
                    Destroy(_skillButtons[i].gameObject);
                }
            }
            _skillButtons.Clear();
        }

        private SkillDefinition[] GetSkillsForCombatant(CombatantState combatant)
        {
            if (combatant == null)
            {
                return null;
            }

            if (_classDefsByHeroId != null
                && _classDefsByHeroId.TryGetValue(combatant.DefinitionId, out var classDef))
            {
                return classDef.StartingSkills;
            }

            return null;
        }

        private Button CreateSkillButton(SkillDefinition skill, Transform parent)
        {
            var buttonRoot = CreatePanel("SkillButton", parent, new Color(0.24f, 0.36f, 0.52f, 1f));
            var button = buttonRoot.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = buttonRoot.GetComponent<Image>();

            var layout = buttonRoot.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = 72;
            layout.preferredHeight = 72;

            var icon = ResolveSkillIcon(skill);
            if (icon != null)
            {
                var iconRect = CreateRectTransform("Icon", buttonRoot);
                StretchToFill(iconRect);
                var iconImage = iconRect.gameObject.AddComponent<Image>();
                iconImage.sprite = icon;
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;
            }
            else
            {
                var text = CreateLabel(skill.DisplayName, buttonRoot, 10, TextAnchor.MiddleCenter);
                text.raycastTarget = false;
            }

            var capturedSkill = skill;
            button.onClick.AddListener(() => OnSkillPressed(capturedSkill));
            return button;
        }

        private Sprite ResolveSkillIcon(SkillDefinition skill)
        {
            if (skill == null)
            {
                return null;
            }

            if (skill.Icon != null)
            {
                return skill.Icon;
            }

            return skill.Id == "basic_strike" ? basicStrikeIcon : null;
        }

        private void OnSkillPressed(SkillDefinition skill)
        {
            if (_combatController == null || _combatController.State != CombatState.PlayerTurn)
            {
                return;
            }

            _activeSkillDefinition = skill;
            _awaitingTargetSelection = true;
            _statusText.text = $"Select a target for {skill.DisplayName}.";
            RefreshTargetSelectionVisuals();
        }

        private sealed class CombatantSlotView
        {
            private readonly CombatSide _side;
            private readonly Image _background;
            private readonly Image _portrait;
            private readonly Image _hpFill;
            private readonly Image _expFill;
            private readonly Button _button;
            private readonly Func<CombatantState, float> _expResolver;

            public CombatantSlotView(
                CombatSide side,
                int rank,
                RectTransform root,
                Button button,
                Image portrait,
                Image hpFill,
                Image expFill,
                Func<CombatantState, float> expResolver)
            {
                Side = side;
                Rank = rank;
                Root = root;
                _button = button;
                _background = root.GetComponent<Image>();
                _portrait = portrait;
                _hpFill = hpFill;
                _expFill = expFill;
                _expResolver = expResolver;
                _side = side;
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
                    Root.gameObject.SetActive(false);
                    _button.interactable = false;
                    return;
                }

                Root.gameObject.SetActive(true);
                _hpFill.fillAmount = Combatant.MaxHp <= 0
                    ? 0f
                    : Mathf.Clamp01((float)Combatant.CurrentHp / Combatant.MaxHp);

                if (_expFill != null)
                {
                    _expFill.transform.parent.gameObject.SetActive(_side == CombatSide.Player);
                    _expFill.fillAmount = _expResolver == null ? 0f : _expResolver(Combatant);
                }

                _portrait.sprite = Portrait;
                _portrait.color = Portrait == null
                    ? new Color(1f, 1f, 1f, 0.15f)
                    : new Color(1f, 1f, 1f, Combatant.IsAlive ? 1f : 0.55f);

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
                    _background.color = ClearSlotColor;
                }

                if (!Combatant.IsAlive)
                {
                    _background.color = ClearSlotColor;
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
