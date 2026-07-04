using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonCrawler.Core.Services
{
    public sealed class SceneLoaderService : ISceneLoaderService
    {
        public bool IsInitialized { get; private set; }

        public Task InitializeAsync()
        {
            IsInitialized = true;

            Debug.Log("[Bootstrap] SceneLoaderService initialized.");
            return Task.CompletedTask;
        }

        public Task LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name cannot be empty.", nameof(sceneName));
            }

            var completion = new TaskCompletionSource<bool>();
            var operation = SceneManager.LoadSceneAsync(sceneName);

            if (operation == null)
            {
                completion.SetException(new InvalidOperationException($"Scene '{sceneName}' could not be loaded."));
                return completion.Task;
            }

            operation.completed += _ =>
            {
                Debug.Log($"[Bootstrap] Loaded scene '{sceneName}'.");
                completion.TrySetResult(true);
            };

            return completion.Task;
        }
    }
}
