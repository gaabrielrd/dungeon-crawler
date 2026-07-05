using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DungeonCrawler.Combat;

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

    public interface ISaveService : IInitializableService
    {
        bool HasLoadedSave { get; }

        SaveSnapshot Current { get; }

        Task<SaveSnapshot> LoadOrCreateAsync();

        Task SaveAsync(SaveSnapshot snapshot);
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

    public interface IDungeonRunService : IInitializableService
    {
        bool HasActiveRun { get; }

        DungeonRunState ActiveRun { get; }

        Task<DungeonRunState> StartRunAsync(string seed = null, List<CombatantState> party = null);

        Task AbandonRunAsync(string runId);

        Task CompleteRunAsync(string runId, Dictionary<string, object> rewards);

        Task<DungeonRunState> LoadRunAsync(string runId);

        void AdvanceFloor();

        void AbandonCurrentRun();

        void CompleteCurrentRun(Dictionary<string, object> rewards);
    }

    public interface IEventBus
    {
        void Publish<TEvent>(TEvent gameEvent);

        void Subscribe<TEvent>(Action<TEvent> handler);

        void Unsubscribe<TEvent>(Action<TEvent> handler);

        void Clear();
    }

    [Serializable]
    public sealed class RunStartedEvent
    {
        public string RunId;
        public string Seed;
        public int CurrentFloor;
        public List<CombatantState> Party;

        public RunStartedEvent(string runId, string seed, int currentFloor, List<CombatantState> party)
        {
            RunId = runId;
            Seed = seed;
            CurrentFloor = currentFloor;
            Party = party;
        }
    }

    [Serializable]
    public sealed class RunAbandonedEvent
    {
        public string RunId;
        public DungeonRunState RunState;

        public RunAbandonedEvent(string runId, DungeonRunState runState)
        {
            RunId = runId;
            RunState = runState;
        }
    }

    [Serializable]
    public sealed class RunCompletedEvent
    {
        public string RunId;
        public DungeonRunState RunState;
        public Dictionary<string, object> Rewards;

        public RunCompletedEvent(string runId, DungeonRunState runState, Dictionary<string, object> rewards)
        {
            RunId = runId;
            RunState = runState;
            Rewards = rewards;
        }
    }

    [Serializable]
    public sealed class FloorAdvancedEvent
    {
        public string RunId;
        public int CurrentFloor;
        public string CurrentThemeId;

        public FloorAdvancedEvent(string runId, int currentFloor, string currentThemeId)
        {
            RunId = runId;
            CurrentFloor = currentFloor;
            CurrentThemeId = currentThemeId;
        }
    }
}
