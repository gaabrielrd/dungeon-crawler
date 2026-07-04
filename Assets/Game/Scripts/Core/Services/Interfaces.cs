using System;
using System.Threading.Tasks;

namespace DungeonCrawler.Core.Services
{
    public interface IInitializableService
    {
        bool IsInitialized { get; }

        Task InitializeAsync();
    }

    public interface IAppConfigService : IInitializableService
    {
        string EnvironmentName { get; }

        string StartingSceneName { get; }
    }

    public interface ILocalSaveService : IInitializableService
    {
        bool HasLoadedSave { get; }

        string ActiveSaveId { get; }
    }

    public interface IAuthService : IInitializableService
    {
        bool IsAuthenticated { get; }

        string PlayerId { get; }
    }

    public interface ISceneLoaderService : IInitializableService
    {
        Task LoadSceneAsync(string sceneName);
    }

    public interface IEventBus
    {
        void Publish<TEvent>(TEvent gameEvent);

        void Subscribe<TEvent>(Action<TEvent> handler);

        void Unsubscribe<TEvent>(Action<TEvent> handler);

        void Clear();
    }
}
