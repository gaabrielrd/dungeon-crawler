using System.Collections.Generic;
using DungeonCrawler.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DungeonCrawler.EditorTools
{
    public static class UIFlowSceneBuilder
    {
        private const string SceneFolder = "Assets/Game/Scenes";
        private static readonly Vector2 ReferenceResolution = new(1080f, 1920f);

        [MenuItem("DungeonCrawler/Build Initial UI Flow Scenes")]
        public static void BuildScenes()
        {
            BuildMainMenuScene();
            BuildRunPreparationScene();
            BuildSettingsScene();
            BuildCombatPrototypeScene();
            UpdateBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void BuildMainMenuScene()
        {
            var screen = CreateBaseScene<MainMenuScreen>(
                "MainMenu",
                "Main Menu",
                "Entry point for starting runs and opening global settings.",
                includeLabels: false);

            var startRunButton = CreateButton(screen.ContentRoot, "Start Run Button", "Start Run", new Vector2(0f, 80f));
            var settingsButton = CreateButton(screen.ContentRoot, "Settings Button", "Settings", new Vector2(0f, -60f));

            SetObjectReference(screen.Screen, "startRunButton", startRunButton);
            SetObjectReference(screen.Screen, "settingsButton", settingsButton);

            SaveScene("MainMenu");
        }

        private static void BuildRunPreparationScene()
        {
            var screen = CreateBaseScene<RunPreparationScreen>(
                "RunPreparation",
                "Run Preparation",
                "Select your party to start the run.",
                includeLabels: false);

            SaveScene("RunPreparation");
        }

        private static void BuildSettingsScene()
        {
            var screen = CreateBaseScene<SettingsScreen>(
                "Settings",
                "Settings",
                "Future home for audio, controls, accessibility, language, and account options.");

            var backButton = CreateButton(screen.ContentRoot, "Back Button", "Back", new Vector2(0f, 60f));

            SetObjectReference(screen.Screen, "backButton", backButton);

            SaveScene("Settings");
        }

        private static void BuildCombatPrototypeScene()
        {
            var screen = CreateBaseScene<CombatPrototypeScreen>(
                "CombatPrototype",
                "Combat Prototype",
                "Temporary target for testing combat UI flow before combat and dungeon systems are connected.");

            var backButton = CreateButton(screen.ContentRoot, "Back To Main Menu Button", "Back to Main Menu", new Vector2(0f, 60f));

            SetObjectReference(screen.Screen, "backToMainMenuButton", backButton);

            SaveScene("CombatPrototype");
        }

        private static SceneBuildContext<TScreen> CreateBaseScene<TScreen>(
            string sceneName,
            string title,
            string description,
            bool includeLabels = true)
            where TScreen : UIScreen
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = sceneName;

            CreateCamera();
            CreateDirectionalLight();

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();

            var canvasObject = new GameObject("Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            var screenObject = new GameObject($"{title} Screen");
            screenObject.transform.SetParent(canvasObject.transform, false);

            var screenRect = screenObject.AddComponent<RectTransform>();
            StretchToParent(screenRect);

            var canvasGroup = screenObject.AddComponent<CanvasGroup>();
            var navigator = screenObject.AddComponent<ScreenNavigator>();
            var screen = screenObject.AddComponent<TScreen>();

            SetObjectReference(screen, "canvasGroup", canvasGroup);
            SetObjectReference(screen, "navigator", navigator);

            var background = screenObject.AddComponent<Image>();
            background.color = new Color(0.06f, 0.07f, 0.09f, 1f);

            var contentRoot = new GameObject("Content");
            contentRoot.transform.SetParent(screenObject.transform, false);

            var contentRect = contentRoot.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.pivot = new Vector2(0.5f, 0.5f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(860f, 760f);

            if (includeLabels)
            {
                CreateText(contentRect, "Title", title, 58, FontStyle.Bold, new Vector2(0f, 250f), new Vector2(860f, 100f));
                CreateText(contentRect, "Purpose Text", description, 32, FontStyle.Normal, new Vector2(0f, 160f), new Vector2(760f, 120f));
            }

            return new SceneBuildContext<TScreen>(screen, contentRect);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.06f, 0.07f, 0.09f, 1f);
            camera.orthographic = true;

            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreateDirectionalLight()
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
        }

        private static Button CreateButton(RectTransform parent, string name, string label, Vector2 anchoredPosition)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            var rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(560f, 96f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.19f, 0.41f, 0.58f, 1f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.normalColor = new Color(0.19f, 0.41f, 0.58f, 1f);
            colors.highlightedColor = new Color(0.27f, 0.53f, 0.72f, 1f);
            colors.pressedColor = new Color(0.13f, 0.28f, 0.42f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.2f, 0.2f, 0.22f, 0.5f);
            button.colors = colors;

            CreateText(rectTransform, "Label", label, 34, FontStyle.Bold, Vector2.zero, new Vector2(520f, 80f));

            return button;
        }

        private static Text CreateText(
            RectTransform parent,
            string name,
            string value,
            int fontSize,
            FontStyle fontStyle,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var rectTransform = textObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            return text;
        }

        private static Font GetBuiltinFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static void StretchToParent(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                Debug.LogError($"Property '{propertyName}' was not found on {target.name}.");
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SaveScene(string sceneName)
        {
            var path = $"{SceneFolder}/{sceneName}.unity";
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path);
        }

        private static void UpdateBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new($"{SceneFolder}/Bootstrap.unity", true),
                new($"{SceneFolder}/MainMenu.unity", true),
                new($"{SceneFolder}/RunPreparation.unity", true),
                new($"{SceneFolder}/Settings.unity", true),
                new($"{SceneFolder}/CombatPrototype.unity", true),
            };

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private readonly struct SceneBuildContext<TScreen>
            where TScreen : UIScreen
        {
            public SceneBuildContext(TScreen screen, RectTransform contentRoot)
            {
                Screen = screen;
                ContentRoot = contentRoot;
            }

            public TScreen Screen { get; }

            public RectTransform ContentRoot { get; }
        }
    }
}
