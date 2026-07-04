using System;
using System.IO;
using System.Threading.Tasks;
using DungeonCrawler.Core.Services;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class AuthAndSaveServiceTests
    {
        private string _temporaryDirectory;

        [SetUp]
        public void SetUp()
        {
            ServiceRegistry.Clear();
            _temporaryDirectory = Path.Combine(Path.GetTempPath(), "DungeonCrawlerSaveTests", Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            ServiceRegistry.Clear();

            if (Directory.Exists(_temporaryDirectory))
            {
                Directory.Delete(_temporaryDirectory, true);
            }
        }

        [Test]
        public async Task MockAuthServiceInitializesAuthenticatedPlayer()
        {
            var authService = new MockAuthService();

            await authService.InitializeAsync();

            Assert.That(authService.IsInitialized, Is.True);
            Assert.That(authService.IsAuthenticated, Is.True);
            Assert.That(authService.PlayerId, Is.EqualTo("local-player"));
        }

        [Test]
        public async Task MockAuthServiceKeepsPlayerIdStableDuringSession()
        {
            var authService = new MockAuthService();

            await authService.InitializeAsync();
            var firstPlayerId = authService.PlayerId;

            await authService.InitializeAsync();

            Assert.That(authService.PlayerId, Is.EqualTo(firstPlayerId));
        }

        [Test]
        public async Task LocalSaveServiceCreatesNewSaveWhenFileDoesNotExist()
        {
            var authService = await CreateInitializedAuthServiceAsync();
            ISaveService saveService = new LocalSaveService(authService, _temporaryDirectory);

            var snapshot = await saveService.LoadOrCreateAsync();

            Assert.That(saveService.HasLoadedSave, Is.True);
            Assert.That(saveService.Current, Is.SameAs(snapshot));
            Assert.That(snapshot.SaveVersion, Is.EqualTo(1));
            Assert.That(snapshot.PlayerId, Is.EqualTo("local-player"));
            Assert.That(snapshot.Profile.SoftCurrency, Is.EqualTo(0));
            Assert.That(snapshot.Profile.PremiumCurrency, Is.EqualTo(0));
            Assert.That(File.Exists(GetSaveFilePath()), Is.True);
        }

        [Test]
        public async Task LocalSaveServiceLoadsExistingSave()
        {
            var existingSnapshot = SaveSnapshot.CreateNew("existing-player");
            existingSnapshot.Profile.SoftCurrency = 25;
            File.WriteAllText(GetSaveFilePath(), JsonUtility.ToJson(existingSnapshot, true));

            var authService = await CreateInitializedAuthServiceAsync();
            ISaveService saveService = new LocalSaveService(authService, _temporaryDirectory);

            var loadedSnapshot = await saveService.LoadOrCreateAsync();

            Assert.That(loadedSnapshot.PlayerId, Is.EqualTo("existing-player"));
            Assert.That(loadedSnapshot.Profile.SoftCurrency, Is.EqualTo(25));
        }

        [Test]
        public async Task LocalSaveServicePersistsUpdatedSnapshot()
        {
            var authService = await CreateInitializedAuthServiceAsync();
            ISaveService saveService = new LocalSaveService(authService, _temporaryDirectory);
            var snapshot = await saveService.LoadOrCreateAsync();
            snapshot.Profile.SoftCurrency = 100;

            await saveService.SaveAsync(snapshot);

            var reloadedService = new LocalSaveService(authService, _temporaryDirectory);
            var reloadedSnapshot = await reloadedService.LoadOrCreateAsync();

            Assert.That(reloadedSnapshot.Profile.SoftCurrency, Is.EqualTo(100));
            Assert.That(reloadedSnapshot.PlayerId, Is.EqualTo("local-player"));
        }

        [Test]
        public async Task SaveServiceCanBeResolvedByInterface()
        {
            var authService = await CreateInitializedAuthServiceAsync();
            ISaveService saveService = new LocalSaveService(authService, _temporaryDirectory);

            ServiceRegistry.Register<ISaveService>(saveService);

            Assert.That(ServiceRegistry.Resolve<ISaveService>(), Is.SameAs(saveService));
        }

        private static async Task<IAuthService> CreateInitializedAuthServiceAsync()
        {
            var authService = new MockAuthService();
            await authService.InitializeAsync();
            return authService;
        }

        private string GetSaveFilePath()
        {
            Directory.CreateDirectory(_temporaryDirectory);
            return Path.Combine(_temporaryDirectory, "save.json");
        }
    }
}
