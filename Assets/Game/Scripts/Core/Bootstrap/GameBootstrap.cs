using System;
using System.Threading.Tasks;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;
using UnityEngine;

namespace DungeonCrawler.Core.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static bool _hasInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetBootstrapState()
        {
            _hasInitialized = false;
        }

        private async void Awake()
        {
            if (_hasInitialized)
            {
                Destroy(gameObject);
                return;
            }

            _hasInitialized = true;
            DontDestroyOnLoad(gameObject);

            try
            {
                await InitializeServicesAsync();
                var configService = ServiceRegistry.Resolve<IAppConfigService>();
                await ServiceRegistry.Resolve<ISceneLoaderService>().LoadSceneAsync(configService.StartingSceneName);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static async Task InitializeServicesAsync()
        {
            ServiceRegistry.Clear();

            var eventBus = new EventBus();
            var appConfigService = new AppConfigService();
            var authService = new MockAuthService();
            var saveService = new LocalSaveService(authService);
            var sceneLoaderService = new SceneLoaderService();
            var dungeonRunService = new DungeonRunService(eventBus);

            ServiceRegistry.Register<IEventBus>(eventBus);
            ServiceRegistry.Register<IAppConfigService>(appConfigService);
            ServiceRegistry.Register<IAuthService>(authService);
            ServiceRegistry.Register<ISaveService>(saveService);
            ServiceRegistry.Register<ISceneLoaderService>(sceneLoaderService);
            ServiceRegistry.Register<IDungeonRunService>(dungeonRunService);

            await appConfigService.InitializeAsync();
            await authService.InitializeAsync();
            await saveService.InitializeAsync();
            await sceneLoaderService.InitializeAsync();
            await dungeonRunService.InitializeAsync();

            Debug.Log("[Bootstrap] Global services initialized.");
        }
    }
}