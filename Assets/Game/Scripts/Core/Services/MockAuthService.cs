using System.Threading.Tasks;
using UnityEngine;

namespace DungeonCrawler.Core.Services
{
    public sealed class MockAuthService : IAuthService
    {
        private readonly string _playerId;

        public MockAuthService()
            : this("local-player")
        {
        }

        public MockAuthService(string playerId)
        {
            _playerId = playerId;
        }

        public bool IsInitialized { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public string PlayerId { get; private set; }

        public Task InitializeAsync()
        {
            if (IsInitialized)
            {
                return Task.CompletedTask;
            }

            PlayerId = _playerId;
            IsAuthenticated = true;
            IsInitialized = true;

            Debug.Log("[Bootstrap] MockAuthService initialized.");
            return Task.CompletedTask;
        }
    }
}
