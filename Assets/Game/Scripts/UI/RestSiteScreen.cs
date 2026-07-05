using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public sealed class RestSiteScreen : UIScreen
    {
        [SerializeField] private ScreenNavigator navigator;

        private IDungeonRunService _dungeonRunService;
        private IRestSiteService _restSiteService;
        private RectTransform _runtimeRoot;
        private Text _headerText;
        private Text _statusText;
        private Button _restButton;
        private Button _continueButton;
        private Button _shopPlaceholder;
        private Button _skillsPlaceholder;
        private Button _upgradesPlaceholder;
        private Button _hirePlaceholder;
        private readonly List<PartyMemberView> _partyViews = new List<PartyMemberView>();

        private static readonly Color PanelBg = new Color(0.08f, 0.09f, 0.12f, 0.92f);
        private static readonly Color ButtonColor = new Color(0.18f, 0.42f, 0.64f, 1f);
        private static readonly Color PlaceholderColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        private static readonly Color DisabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        private static readonly Color AliveColor = new Color(1f, 1f, 1f, 0.98f);
        private static readonly Color DeadColor = new Color(0.75f, 0.75f, 0.75f, 0.95f);
        private static readonly Color HpBarBg = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        private static readonly Color HpBarFill = new Color(0.2f, 0.6f, 0.2f, 0.95f);
        private static readonly Color HpBarLow = new Color(0.7f, 0.2f, 0.2f, 0.95f);

        private void OnEnable()
        {
            ResolveServices();
            SubscribeToEvents();
            EnsureRuntimeUi();
            RefreshPartyDisplay();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
            CleanupRuntimeUi();
        }

        private void ResolveServices()
        {
            if (!ServiceRegistry.TryResolve<IDungeonRunService>(out _dungeonRunService))
                _dungeonRunService = null;

            if (!ServiceRegistry.TryResolve<IRestSiteService>(out _restSiteService))
                _restSiteService = null;
        }

        private void SubscribeToEvents()
        {
        }

        private void UnsubscribeFromEvents()
        {
        }

        private void EnsureRuntimeUi()
        {
            if (_runtimeRoot != null) return;

            _runtimeRoot = CreateRectTransform("RestSite Runtime Root", transform as RectTransform);
            StretchToFill(_runtimeRoot);

            EnsureCanvasScaler();

            var rootLayout = _runtimeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(12, 12, 12, 12);
            rootLayout.spacing = 8;
            rootLayout.childControlHeight = true;
            rootLayout.childControlWidth = true;
            rootLayout.childForceExpandHeight = false;
            rootLayout.childForceExpandWidth = true;

            CreateHeaderArea();
            CreatePartyArea();
            CreateActionArea();
            CreatePlaceholderArea();
            CreateStatusBar();
        }

        private void CreateHeaderArea()
        {
            var section = CreatePanel("Header", _runtimeRoot, PanelBg);
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 4;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.preferredHeight = 90;

            var floorLabel = "--";
            if (_dungeonRunService?.ActiveRun?.CurrentFloorInfo != null)
                floorLabel = _dungeonRunService.ActiveRun.CurrentFloorInfo.FloorNumber.ToString();

            _headerText = CreateLabel($"Safe Resting Site - Floor {floorLabel}", section, 28, TextAnchor.MiddleCenter);

            var subtext = CreateLabel("A safe haven between dungeon depths.", section, 16, TextAnchor.MiddleCenter);
            subtext.color = new Color(0.7f, 0.7f, 0.7f, 0.9f);
        }

        private void CreatePartyArea()
        {
            var section = CreatePanel("Party Status", _runtimeRoot, PanelBg);
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.spacing = 6;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.flexibleHeight = 1;

            CreateLabel("Party Status", section, 20, TextAnchor.MiddleLeft);

            var party = _dungeonRunService?.ActiveRun?.Party;
            if (party != null)
            {
                for (var i = 0; i < party.Count; i++)
                {
                    var member = party[i];
                    var row = CreatePartyMemberRow(section, member, i);
                    _partyViews.Add(row);
                }
            }
        }

        private PartyMemberView CreatePartyMemberRow(Transform parent, CombatantState member, int index)
        {
            var row = CreatePanel($"Member {index}", parent, new Color(0.12f, 0.13f, 0.17f, 0.85f));
            var rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.padding = new RectOffset(8, 8, 4, 4);
            rowLayout.spacing = 8;
            rowLayout.childControlHeight = true;
            rowLayout.childControlWidth = true;
            rowLayout.childForceExpandHeight = false;
            rowLayout.childForceExpandWidth = false;

            var rowSize = row.gameObject.AddComponent<LayoutElement>();
            rowSize.preferredHeight = 48;
            rowSize.flexibleWidth = 1;

            var nameText = CreateLabel(member.DisplayName, row, 16, TextAnchor.MiddleLeft);
            var nameSize = nameText.gameObject.AddComponent<LayoutElement>();
            nameSize.preferredWidth = 120;

            var hpLabel = $"{member.CurrentHp}/{member.MaxHp}";
            var aliveLabel = member.IsAlive ? "Alive" : "Defeated";
            var hpText = CreateLabel($"{hpLabel}  {aliveLabel}", row, 14, TextAnchor.MiddleLeft);
            var hpSize = hpText.gameObject.AddComponent<LayoutElement>();
            hpSize.preferredWidth = 100;

            var barRoot = CreateRectTransform("HP Bar", row);
            var barLayout = barRoot.gameObject.AddComponent<LayoutElement>();
            barLayout.flexibleWidth = 1;
            barLayout.preferredHeight = 18;

            var barBg = barRoot.gameObject.AddComponent<Image>();
            barBg.color = HpBarBg;

            var fillRect = CreateRectTransform("HP Fill", barRoot);
            StretchToFill(fillRect);
            var fillImage = fillRect.gameObject.AddComponent<Image>();
            var fillPct = member.MaxHp > 0 ? (float)member.CurrentHp / member.MaxHp : 0f;
            fillImage.color = fillPct > 0.3f ? HpBarFill : HpBarLow;
            fillRect.anchorMax = new Vector2(fillPct, 1f);

            return new PartyMemberView(nameText, hpText, fillImage);
        }

        private void CreateActionArea()
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

            _restButton = CreateTextButton("Rest", section, OnRestPressed, ButtonColor);
            _continueButton = CreateTextButton("Continue", section, OnContinuePressed, ButtonColor);
            _continueButton.interactable = false;
        }

        private void CreatePlaceholderArea()
        {
            var section = CreatePanel("Extras", _runtimeRoot, PanelBg);
            var layout = section.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.preferredHeight = 70;

            _shopPlaceholder = CreateTextButton("Shop", section, null, PlaceholderColor);
            _shopPlaceholder.interactable = false;
            _skillsPlaceholder = CreateTextButton("Skills", section, null, PlaceholderColor);
            _skillsPlaceholder.interactable = false;
            _upgradesPlaceholder = CreateTextButton("Upgrades", section, null, PlaceholderColor);
            _upgradesPlaceholder.interactable = false;
            _hirePlaceholder = CreateTextButton("Hire", section, null, PlaceholderColor);
            _hirePlaceholder.interactable = false;
        }

        private void CreateStatusBar()
        {
            var section = CreatePanel("Status", _runtimeRoot, PanelBg);
            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            var size = section.gameObject.AddComponent<LayoutElement>();
            size.preferredHeight = 60;

            _statusText = CreateLabel("Rest and prepare for the next depths.", section, 18, TextAnchor.MiddleCenter);
        }

        private void RefreshPartyDisplay()
        {
            var party = _dungeonRunService?.ActiveRun?.Party;
            if (party == null) return;

            for (var i = 0; i < _partyViews.Count && i < party.Count; i++)
            {
                var member = party[i];
                var view = _partyViews[i];

                var hpLabel = $"{member.CurrentHp}/{member.MaxHp}";
                var aliveLabel = member.IsAlive ? "Alive" : "Defeated";
                view.HpText.text = $"{hpLabel}  {aliveLabel}";

                var fillPct = member.MaxHp > 0 ? (float)member.CurrentHp / member.MaxHp : 0f;
                view.HpFill.color = fillPct > 0.3f ? HpBarFill : HpBarLow;
                view.HpFill.rectTransform.anchorMax = new Vector2(fillPct, 1f);
            }
        }

        private void OnRestPressed()
        {
            if (_restSiteService == null || _dungeonRunService?.ActiveRun?.Party == null) return;

            try
            {
                _restSiteService.Rest(_dungeonRunService.ActiveRun.Party);
                RefreshPartyDisplay();
                _restButton.interactable = false;
                _continueButton.interactable = true;
                _statusText.text = "Party has rested. You may continue when ready.";
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                _statusText.text = "Failed to rest.";
            }
        }

        private void OnContinuePressed()
        {
            if (_dungeonRunService == null || !_dungeonRunService.HasActiveRun) return;

            try
            {
                _dungeonRunService.AdvanceFloor();
                _restSiteService?.ResetRestTaken();

                if (navigator != null)
                {
                    navigator.GoToCombatPrototype();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                _statusText.text = "Failed to continue.";
            }
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

            _partyViews.Clear();
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
            label.color = AliveColor;
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
            layout.preferredWidth = 160;
            layout.preferredHeight = 52;

            var text = CreateLabel(label, buttonRoot, 18, TextAnchor.MiddleCenter);
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

        private sealed class PartyMemberView
        {
            public Text NameText { get; }
            public Text HpText { get; }
            public Image HpFill { get; }

            public PartyMemberView(Text nameText, Text hpText, Image hpFill)
            {
                NameText = nameText;
                HpText = hpText;
                HpFill = hpFill;
            }
        }
    }
}
