using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DungeonCrawler.Core.Services
{
    public sealed class AuthService : IAuthService
    {
        public bool IsInitialized { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public string PlayerId { get; private set; }

        public Task InitializeAsync()
        {
            PlayerId = $"mock_player_{Guid.NewGuid():N}";
            IsAuthenticated = true;
            IsInitialized = true;

            Debug.Log("[Bootstrap] AuthService mock initialized.");
            return Task.CompletedTask;
        }
    }
}
