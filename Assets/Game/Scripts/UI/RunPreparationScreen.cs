using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Data.State;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class RunPreparationScreen : UIScreen
    {
        private const int MaxSelectionCount = 2;
        private const int TotalHeroCount = 4;

        [SerializeField] private ScreenNavigator navigator;
        [SerializeField] private TestCombatDataProvider testData;
        [SerializeField] private Sprite[] heroPortraits;
        [SerializeField] private DungeonThemeDefinition themeDefinition;

        private RectTransform _runtimeRoot;
        private Button _startRunButton;
        private Text _statusText;
        private readonly bool[] _selected = new bool[TotalHeroCount];
        private readonly List<HeroCardView> _heroCards = new List<HeroCardView>();

        private static readonly Color PanelBg = new Color(0.08f, 0.09f, 0.12f, 0.92f);
        private static readonly Color CardNormal = new Color(0.12f, 0.13f, 0.17f, 0.85f);
        private static readonly Color CardSelected = new Color(0.18f, 0.42f, 0.64f, 0.95f);
        private static readonly Color CardHover = new Color(0.2f, 0.22f, 0.28f, 0.9f);
        private static readonly Color ButtonColor = new Color(0.18f, 0.42f, 0.64f, 1f);
        private static readonly Color ButtonDisabled = new Color(0.2f, 0.2f, 0.22f, 0.5f);
        private static readonly Color EnabledTextColor = new Color(1f, 1f, 1f, 0.98f);

        private void Awake()
        {
            ResolveNavigator();
        }

        private void OnEnable()
        {
            ResolveNavigator();
            AutoLoadAssets();
            EnsureRuntimeUi();
        }

        private void OnDisable()
        {
            CleanupRuntimeUi();
        }

        private void ResolveNavigator()
        {
            if (navigator == null)
                navigator = GetComponent<ScreenNavigator>();
        }

        private void AutoLoadAssets()
        {
#if UNITY_EDITOR
            if (testData == null)
            {
                var path = "Assets/Game/Data/Test/test_combat_data.asset";
                testData = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(path);
            }

            if (heroPortraits == null || heroPortraits.Length < TotalHeroCount)
            {
                heroPortraits = new Sprite[TotalHeroCount];
                for (var i = 0; i < TotalHeroCount; i++)
                {
                    var portraitPath = $"Assets/Game/Art/Heroes/hero_{i + 1}.png";
                    heroPortraits[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(portraitPath);
                }
            }
#endif
        }

        private void EnsureRuntimeUi()
        {
            if (_runtimeRoot != null) return;

            _runtimeRoot = CreateRectTransform("RunPreparation Runtime Root", transform as RectTransform);
            StretchToFill(_runtimeRoot);

            EnsureCanvasScaler();

            var rootLayout = _runtimeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(12, 12, 12, 12);
            rootLayout.spacing = 4;
            rootLayout.childControlHeight = true;
            rootLayout.childControlWidth = true;
            rootLayout.childForceExpandHeight = false;
            rootLayout.childForceExpandWidth = true;

            CreateHeader();
            CreateHeroSelectionArea();
            CreateActionButtons();
        }

        private void CreateHeader()
        {
            var section = CreatePanel("Header", _runtimeRoot, Color.clear);
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 4, 0);
            layout.spacing = 2;
            layout.childAlignment = TextAnchor.LowerCenter;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.preferredHeight = 56;

            var title = CreateLabel("Select Your Party", section, 24, TextAnchor.LowerCenter);
            title.fontStyle = FontStyle.Bold;

            _statusText = CreateLabel("Select 2 heroes to start the run.", section, 14, TextAnchor.LowerCenter);
            _statusText.color = new Color(0.7f, 0.7f, 0.7f, 0.9f);
        }

        private void CreateHeroSelectionArea()
        {
            var section = CreatePanel("Hero Selection", _runtimeRoot, Color.clear);
            var layout = section.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 0);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.LowerCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.flexibleHeight = 1;

            if (testData == null || testData.HeroDefinitions == null)
            {
                var errorLabel = CreateLabel("No hero data available.", section, 18, TextAnchor.MiddleCenter);
                errorLabel.color = Color.red;
                return;
            }

            var count = Mathf.Min(TotalHeroCount, testData.HeroDefinitions.Length);
            for (var i = 0; i < count; i++)
            {
                var heroDef = testData.HeroDefinitions[i];
                if (heroDef == null) continue;
                var portrait = heroPortraits != null && i < heroPortraits.Length ? heroPortraits[i] : null;
                var card = CreateHeroCard(section, heroDef, portrait, i);
                _heroCards.Add(card);
            }
        }

        private HeroCardView CreateHeroCard(Transform parent, HeroClassDefinition heroDef, Sprite portrait, int index)
        {
            var cardRoot = CreatePanel($"Hero Card {index}", parent, CardNormal);
            var cardLayout = cardRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.padding = new RectOffset(4, 4, 4, 4);
            cardLayout.spacing = 2;
            cardLayout.childAlignment = TextAnchor.LowerCenter;
            cardLayout.childControlHeight = true;
            cardLayout.childControlWidth = true;
            cardLayout.childForceExpandHeight = false;
            cardLayout.childForceExpandWidth = true;

            var cardSize = cardRoot.gameObject.AddComponent<LayoutElement>();
            cardSize.flexibleWidth = 1;
            cardSize.preferredHeight = 240;

            var button = cardRoot.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = cardRoot.GetComponent<Image>();

            var colors = button.colors;
            colors.normalColor = CardNormal;
            colors.highlightedColor = CardHover;
            colors.pressedColor = CardSelected;
            colors.selectedColor = CardSelected;
            colors.disabledColor = CardNormal;
            button.colors = colors;

            if (portrait != null)
            {
                var portraitRect = CreateRectTransform("Portrait", cardRoot);
                var portraitImage = portraitRect.gameObject.AddComponent<Image>();
                portraitImage.sprite = portrait;
                portraitImage.preserveAspect = true;
                portraitImage.raycastTarget = false;

                var portraitLayout = portraitRect.gameObject.AddComponent<LayoutElement>();
                portraitLayout.flexibleWidth = 1;
                portraitLayout.flexibleHeight = 1;
                portraitLayout.preferredHeight = 120;
                portraitLayout.minHeight = 80;
            }

            var className = CreateLabel(heroDef.DisplayName, cardRoot, 16, TextAnchor.MiddleCenter);
            className.fontStyle = FontStyle.Bold;
            className.raycastTarget = false;

            var classLayout = className.gameObject.AddComponent<LayoutElement>();
            classLayout.preferredHeight = 24;

            var skillsText = BuildSkillsText(heroDef.StartingSkills);
            var skillLabel = CreateLabel(skillsText, cardRoot, 12, TextAnchor.MiddleCenter);
            skillLabel.raycastTarget = false;
            skillLabel.color = new Color(0.7f, 0.7f, 0.7f, 0.9f);

            var skillLayout = skillLabel.gameObject.AddComponent<LayoutElement>();
            skillLayout.preferredHeight = 40;

            var capturedIndex = index;
            button.onClick.AddListener(() => OnHeroCardClicked(capturedIndex));

            return new HeroCardView(cardRoot);
        }

        private static string BuildSkillsText(SkillDefinition[] skills)
        {
            if (skills == null || skills.Length == 0)
                return "No skills";

            var names = new string[skills.Length];
            for (var i = 0; i < skills.Length; i++)
            {
                names[i] = skills[i] != null ? skills[i].DisplayName : "Unknown";
            }
            return string.Join(", ", names);
        }

        private void CreateActionButtons()
        {
            var section = CreatePanel("Actions", _runtimeRoot, PanelBg);
            var layout = section.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.spacing = 12;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.preferredHeight = 80;

            CreateTextButton("Back", section, OnBackPressed, ButtonColor);
            _startRunButton = CreateTextButton("Start Run", section, OnStartRunPressed, ButtonColor);
            _startRunButton.interactable = false;
        }

        private void OnHeroCardClicked(int index)
        {
            if (index < 0 || index >= _selected.Length)
                return;

            if (_selected[index])
            {
                _selected[index] = false;
                _heroCards[index].Root.GetComponent<Image>().color = CardNormal;
                UpdateStatusText();
                return;
            }

            var selectedCount = CountSelected();
            if (selectedCount >= MaxSelectionCount)
            {
                _statusText.text = "Maximum 2 heroes selected.";
                return;
            }

            _selected[index] = true;
            _heroCards[index].Root.GetComponent<Image>().color = CardSelected;
            UpdateStatusText();
        }

        private int CountSelected()
        {
            var count = 0;
            for (var i = 0; i < _selected.Length; i++)
            {
                if (_selected[i]) count++;
            }
            return count;
        }

        private void UpdateStatusText()
        {
            var count = CountSelected();
            if (count == 0)
            {
                _statusText.text = "Select 2 heroes to start the run.";
                _startRunButton.interactable = false;
            }
            else if (count == 1)
            {
                _statusText.text = "Select 1 more hero.";
                _startRunButton.interactable = false;
            }
            else
            {
                _statusText.text = "Ready! Press Start Run to begin.";
                _startRunButton.interactable = true;
            }
        }

        private void OnBackPressed()
        {
            if (navigator != null)
                navigator.GoToMainMenu();
        }

        private async void OnStartRunPressed()
        {
            if (!ServiceRegistry.TryResolve<IDungeonRunService>(out var dungeonRunService))
            {
                _statusText.text = "Service not available.";
                return;
            }

            _startRunButton.interactable = false;

            var selectedIndices = GetSelectedIndices();
            if (selectedIndices.Count != MaxSelectionCount)
            {
                _statusText.text = "Select exactly 2 heroes.";
                return;
            }

            try
            {
                var party = new List<CombatantState>();
                var roster = new List<HeroState>();

                for (var i = 0; i < selectedIndices.Count; i++)
                {
                    var def = testData.HeroDefinitions[selectedIndices[i]];
                    var heroState = new HeroState(def, def.DisplayName, Rarity.Common);
                    heroState.PartyRank = i + 1;
                    heroState.IsInParty = true;
                    roster.Add(heroState);
                    party.Add(CombatantStateFactory.CreateHeroFromState(heroState, i + 1));
                }

                if (dungeonRunService is DungeonRunService concreteService && themeDefinition != null)
                {
                    concreteService.CurrentThemeDefinition = themeDefinition;
                }

                if (!dungeonRunService.HasActiveRun)
                {
                    await dungeonRunService.StartRunAsync(party: party);
                    if (dungeonRunService.ActiveRun != null)
                    {
                        dungeonRunService.ActiveRun.Roster = roster;
                    }
                }

                if (navigator != null)
                    navigator.GoToCombatPrototype();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                _statusText.text = "Failed to start run.";
                UpdateStatusText();
            }
        }

        private List<int> GetSelectedIndices()
        {
            var indices = new List<int>();
            for (var i = 0; i < _selected.Length; i++)
            {
                if (_selected[i]) indices.Add(i);
            }
            return indices;
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

        private void CleanupRuntimeUi()
        {
            if (_runtimeRoot != null)
            {
                Destroy(_runtimeRoot.gameObject);
                _runtimeRoot = null;
            }

            _heroCards.Clear();
            Array.Clear(_selected, 0, _selected.Length);
        }

        private static RectTransform CreateRectTransform(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            return rect;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color bgColor)
        {
            var panel = CreateRectTransform(name, parent);
            var image = panel.gameObject.AddComponent<Image>();
            image.color = bgColor;
            return panel;
        }

        private static Text CreateLabel(string text, Transform parent, int size, TextAnchor anchor)
        {
            var rect = CreateRectTransform("Label", parent);
            var label = rect.gameObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = text;
            label.fontSize = size;
            label.alignment = anchor;
            label.color = EnabledTextColor;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;

            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = size + 8;
            layout.flexibleWidth = 1;
            return label;
        }

        private static Button CreateTextButton(string label, Transform parent, Action callback, Color? colorOverride = null)
        {
            var bgColor = colorOverride ?? new Color(0.18f, 0.42f, 0.64f, 1f);
            var buttonRoot = CreatePanel("Button", parent, bgColor);
            var button = buttonRoot.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = buttonRoot.GetComponent<Image>();

            var layout = buttonRoot.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = 260;
            layout.preferredHeight = 64;

            var text = CreateLabel(label, buttonRoot, 22, TextAnchor.MiddleCenter);
            text.raycastTarget = false;

            if (callback != null)
                button.onClick.AddListener(() => callback());

            return button;
        }

        private static void StretchToFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private sealed class HeroCardView
        {
            public RectTransform Root { get; }

            public HeroCardView(RectTransform root)
            {
                Root = root;
            }
        }
    }
}
