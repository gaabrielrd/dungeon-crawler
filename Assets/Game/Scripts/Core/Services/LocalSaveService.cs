using System.Threading.Tasks;
using UnityEngine;

namespace DungeonCrawler.Core.Services
{
    public sealed class LocalSaveService : ILocalSaveService
    {
        public bool IsInitialized { get; private set; }

        public bool HasLoadedSave { get; private set; }

        public string ActiveSaveId { get; private set; }

        public Task InitializeAsync()
        {
            ActiveSaveId = "local_mock_save";
            HasLoadedSave = true;
            IsInitialized = true;

            Debug.Log("[Bootstrap] LocalSaveService mock initialized.");
            return Task.CompletedTask;
        }
    }
}
