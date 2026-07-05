using System.Threading.Tasks;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Dungeon;
using DungeonCrawler.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class UIFlowNavigationTests
    {
        [SetUp]
        public void SetUp()
        {
            ServiceRegistry.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceRegistry.Clear();
        }

        [Test]
        public void MainMenuStartRunButtonNavigatesToRunPreparation()
        {
            var sceneLoader = new MockSceneLoaderService();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoader);

            var fixture = CreateScreenFixture<MainMenuScreen>();
            var startRunButton = CreateButton("Start Run Button");
            var settingsButton = CreateButton("Settings Button");

            SetObjectReference(fixture.Screen, "startRunButton", startRunButton);
            SetObjectReference(fixture.Screen, "settingsButton", settingsButton);

            EnableFixture(fixture);
            startRunButton.onClick.Invoke();

            Assert.That(sceneLoader.LastLoadedSceneName, Is.EqualTo("RunPreparation"));

            DestroyFixture(fixture.Root, startRunButton.gameObject, settingsButton.gameObject);
        }

        [Test]
        public void MainMenuSettingsButtonNavigatesToSettings()
        {
            var sceneLoader = new MockSceneLoaderService();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoader);

            var fixture = CreateScreenFixture<MainMenuScreen>();
            var startRunButton = CreateButton("Start Run Button");
            var settingsButton = CreateButton("Settings Button");

            SetObjectReference(fixture.Screen, "startRunButton", startRunButton);
            SetObjectReference(fixture.Screen, "settingsButton", settingsButton);

            EnableFixture(fixture);
            settingsButton.onClick.Invoke();

            Assert.That(sceneLoader.LastLoadedSceneName, Is.EqualTo("Settings"));

            DestroyFixture(fixture.Root, startRunButton.gameObject, settingsButton.gameObject);
        }

        [Test]
        public void RunPreparationStartCombatButtonNavigatesToCombatPrototype()
        {
            var sceneLoader = new MockSceneLoaderService();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoader);

            var fixture = CreateScreenFixture<RunPreparationScreen>();
            var startCombatButton = CreateButton("Start Combat Button");
            var backButton = CreateButton("Back Button");

            SetObjectReference(fixture.Screen, "startCombatButton", startCombatButton);
            SetObjectReference(fixture.Screen, "backButton", backButton);

            EnableFixture(fixture);
            startCombatButton.onClick.Invoke();

            Assert.That(sceneLoader.LastLoadedSceneName, Is.EqualTo("CombatPrototype"));

            DestroyFixture(fixture.Root, startCombatButton.gameObject, backButton.gameObject);
        }

        [Test]
        public void RunPreparationStartCombatButtonStartsRunBeforeNavigating()
        {
            var sceneLoader = new MockSceneLoaderService();
            var dungeonRunService = new MockDungeonRunService();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoader);
            ServiceRegistry.Register<IDungeonRunService>(dungeonRunService);

            var fixture = CreateScreenFixture<RunPreparationScreen>();
            var startCombatButton = CreateButton("Start Combat Button");
            var backButton = CreateButton("Back Button");

            SetObjectReference(fixture.Screen, "startCombatButton", startCombatButton);
            SetObjectReference(fixture.Screen, "backButton", backButton);

            EnableFixture(fixture);
            startCombatButton.onClick.Invoke();

            Assert.That(dungeonRunService.StartRunCallCount, Is.EqualTo(1));
            Assert.That(sceneLoader.LastLoadedSceneName, Is.EqualTo("CombatPrototype"));

            DestroyFixture(fixture.Root, startCombatButton.gameObject, backButton.gameObject);
        }

        [Test]
        public void SettingsBackButtonNavigatesToMainMenu()
        {
            var sceneLoader = new MockSceneLoaderService();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoader);

            var fixture = CreateScreenFixture<SettingsScreen>();
            var backButton = CreateButton("Back Button");

            SetObjectReference(fixture.Screen, "backButton", backButton);

            EnableFixture(fixture);
            backButton.onClick.Invoke();

            Assert.That(sceneLoader.LastLoadedSceneName, Is.EqualTo("MainMenu"));

            DestroyFixture(fixture.Root, backButton.gameObject);
        }

        [Test]
        public void CombatPrototypeBackButtonNavigatesToMainMenu()
        {
            var sceneLoader = new MockSceneLoaderService();
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoader);

            var fixture = CreateScreenFixture<CombatPrototypeScreen>();
            var backButton = CreateButton("Back To Main Menu Button");

            SetObjectReference(fixture.Screen, "backToMainMenuButton", backButton);

            EnableFixture(fixture);
            backButton.onClick.Invoke();

            Assert.That(sceneLoader.LastLoadedSceneName, Is.EqualTo("MainMenu"));

            DestroyFixture(fixture.Root, backButton.gameObject);
        }

        private static ScreenFixture<TScreen> CreateScreenFixture<TScreen>()
            where TScreen : UIScreen
        {
            var root = new GameObject(typeof(TScreen).Name);
            root.SetActive(false);

            var navigator = root.AddComponent<ScreenNavigator>();
            var screen = root.AddComponent<TScreen>();

            SetObjectReference(screen, "navigator", navigator);

            return new ScreenFixture<TScreen>(root, screen);
        }

        private static Button CreateButton(string name)
        {
            var gameObject = new GameObject(name);
            gameObject.AddComponent<RectTransform>();
            gameObject.AddComponent<Image>();
            return gameObject.AddComponent<Button>();
        }

        private static void EnableFixture<TScreen>(ScreenFixture<TScreen> fixture)
            where TScreen : UIScreen
        {
            InvokePrivateMethod(fixture.Screen, "OnEnable");
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            var field = target.GetType().GetField(
                propertyName,
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public);

            Assert.That(field, Is.Not.Null, $"Field '{propertyName}' was not found on {target.GetType().Name}.");

            field.SetValue(target, value);
        }

        private static void InvokePrivateMethod(Object target, string methodName)
        {
            var method = target.GetType().GetMethod(
                methodName,
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public);

            Assert.That(method, Is.Not.Null, $"Method '{methodName}' was not found on {target.GetType().Name}.");

            method.Invoke(target, null);
        }

        private static void DestroyFixture(GameObject root, params GameObject[] additionalObjects)
        {
            Object.DestroyImmediate(root);

            foreach (var additionalObject in additionalObjects)
            {
                Object.DestroyImmediate(additionalObject);
            }
        }

        private readonly struct ScreenFixture<TScreen>
            where TScreen : UIScreen
        {
            public ScreenFixture(GameObject root, TScreen screen)
            {
                Root = root;
                Screen = screen;
            }

            public GameObject Root { get; }

            public TScreen Screen { get; }
        }

        private sealed class MockSceneLoaderService : ISceneLoaderService
        {
            public bool IsInitialized => true;

            public string LastLoadedSceneName { get; private set; }

            public Task InitializeAsync()
            {
                return Task.CompletedTask;
            }

            public Task LoadSceneAsync(string sceneName)
            {
                LastLoadedSceneName = sceneName;
                return Task.CompletedTask;
            }
        }

        private sealed class MockDungeonRunService : IDungeonRunService
        {
            public bool IsInitialized => true;

            public bool HasActiveRun { get; private set; }

            public DungeonRunState ActiveRun { get; private set; }

            public CombatController CurrentCombatController => null;

            public int StartRunCallCount { get; private set; }

            public Task InitializeAsync()
            {
                return Task.CompletedTask;
            }

            public Task<DungeonRunState> StartRunAsync(string seed = null, System.Collections.Generic.List<CombatantState> party = null)
            {
                StartRunCallCount++;
                ActiveRun = DungeonRunState.CreateNew(seed, party);
                ActiveRun.Status = DungeonRunStatus.Exploring;
                HasActiveRun = true;
                return Task.FromResult(ActiveRun);
            }

            public Task AbandonRunAsync(string runId)
            {
                HasActiveRun = false;
                ActiveRun = null;
                return Task.CompletedTask;
            }

            public Task CompleteRunAsync(string runId, System.Collections.Generic.Dictionary<string, object> rewards)
            {
                HasActiveRun = false;
                ActiveRun = null;
                return Task.CompletedTask;
            }

            public Task<DungeonRunState> LoadRunAsync(string runId)
            {
                return Task.FromResult(ActiveRun);
            }

            public GeneratedFloor GenerateCurrentFloor()
            {
                return null;
            }

            public CombatController StartCurrentFloorCombat(CombatFormationState formation)
            {
                return null;
            }

            public void ResolveCurrentCombatResult(CombatState result)
            {
            }

            public void AdvanceFloor()
            {
            }

            public void AbandonCurrentRun()
            {
                HasActiveRun = false;
                ActiveRun = null;
            }

            public void CompleteCurrentRun(System.Collections.Generic.Dictionary<string, object> rewards)
            {
                HasActiveRun = false;
                ActiveRun = null;
            }
        }
    }
}
