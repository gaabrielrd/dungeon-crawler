using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DungeonCrawler.Core.Services
{
    public sealed class LocalSaveService : ISaveService
    {
        private const string SaveFileName = "save.json";

        private readonly IAuthService _authService;
        private readonly string _saveDirectory;

        public LocalSaveService(IAuthService authService)
            : this(authService, Application.persistentDataPath)
        {
        }

        public LocalSaveService(IAuthService authService, string saveDirectory)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _saveDirectory = saveDirectory ?? throw new ArgumentNullException(nameof(saveDirectory));
        }

        public bool IsInitialized { get; private set; }

        public bool HasLoadedSave { get; private set; }

        public SaveSnapshot Current { get; private set; }

        private string SaveFilePath => Path.Combine(_saveDirectory, SaveFileName);

        public async Task InitializeAsync()
        {
            if (IsInitialized)
            {
                return;
            }

            await LoadOrCreateAsync();
            IsInitialized = true;

            Debug.Log("[Bootstrap] LocalSaveService initialized.");
        }

        public async Task<SaveSnapshot> LoadOrCreateAsync()
        {
            Directory.CreateDirectory(_saveDirectory);

            if (File.Exists(SaveFilePath))
            {
                var json = File.ReadAllText(SaveFilePath);
                Current = JsonUtility.FromJson<SaveSnapshot>(json);
                Current.Normalize();
                HasLoadedSave = true;

                return Current;
            }

            Current = SaveSnapshot.CreateNew(_authService.PlayerId);
            HasLoadedSave = true;
            await SaveAsync(Current);

            return Current;
        }

        public Task SaveAsync(SaveSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            Directory.CreateDirectory(_saveDirectory);

            snapshot.Normalize();
            snapshot.LastUpdatedUtc = DateTime.UtcNow.ToString("O");

            var json = JsonUtility.ToJson(snapshot, true);
            File.WriteAllText(SaveFilePath, json);

            Current = snapshot;
            HasLoadedSave = true;

            return Task.CompletedTask;
        }
    }
}
