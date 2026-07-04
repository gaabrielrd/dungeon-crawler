using System.Threading.Tasks;
using UnityEngine;

namespace DungeonCrawler.Core.Services
{
    public sealed class AppConfigService : IAppConfigService
    {
        public bool IsInitialized { get; private set; }

        public string EnvironmentName { get; private set; }

        public string StartingSceneName { get; private set; }

        public Task InitializeAsync()
        {
            EnvironmentName = Debug.isDebugBuild ? "Development" : "Production";
            StartingSceneName = "MainMenu";
            IsInitialized = true;

            Debug.Log($"[Bootstrap] AppConfigService initialized for {EnvironmentName}.");
            return Task.CompletedTask;
        }
    }
}
