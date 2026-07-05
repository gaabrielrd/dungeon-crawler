using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;

namespace DungeonCrawler.Core.Services
{
    public sealed class DungeonRunService : IDungeonRunService
    {
        private readonly IEventBus _eventBus;

        public DungeonRunService(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public bool IsInitialized { get; private set; }

        public bool HasActiveRun { get; private set; }

        public DungeonRunState ActiveRun { get; private set; }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
            IsInitialized = true;
        }

        public async Task<DungeonRunState> StartRunAsync(string seed = null, List<CombatantState> party = null)
        {
            if (HasActiveRun)
            {
                throw new InvalidOperationException("Run already in progress.");
            }

            var run = DungeonRunState.CreateNew(seed, party);
            ActiveRun = run;
            HasActiveRun = true;

            _eventBus.Publish(new RunStartedEvent(run.RunId, run.Seed, run.CurrentFloor, run.Party));

            return run;
        }

        public async Task AbandonRunAsync(string runId)
        {
            if (!HasActiveRun || ActiveRun.RunId != runId)
            {
                throw new InvalidOperationException("No active run or run not found.");
            }

            HasActiveRun = false;
            var abandonedRun = ActiveRun;
            ActiveRun = null;

            _eventBus.Publish(new RunAbandonedEvent(runId, abandonedRun));
        }

        public async Task CompleteRunAsync(string runId, Dictionary<string, object> rewards)
        {
            if (!HasActiveRun || ActiveRun.RunId != runId)
            {
                throw new InvalidOperationException("No active run or run not found.");
            }

            var completedRun = ActiveRun;
            completedRun.inventorySnapshot = new SaveProfileSnapshot();
            completedRun.visitedFloors.Add(completedRun.CurrentFloor);

            HasActiveRun = false;
            _eventBus.Publish(new RunCompletedEvent(runId, completedRun, rewards));
        }

        public async Task<DungeonRunState> LoadRunAsync(string runId)
        {
            var run = new DungeonRunState
            {
                runId = runId,
                seed = "test-seed-123",
                currentFloor = 5,
                currentThemeId = "crypt",
                party = new List<CombatantState>(),
                inventorySnapshot = new SaveProfileSnapshot(),
                visitedFloors = new HashSet<int> { 1, 2, 3, 4, 5 },
                startedAtUtc = DateTime.UtcNow.AddDays(-2).ToString("O")
            };

            return run;
        }

        public void AdvanceFloor()
        {
            if (!HasActiveRun)
            {
                throw new InvalidOperationException("No active run to advance.");
            }

            ActiveRun.AdvanceFloor();
            _eventBus.Publish(new FloorAdvancedEvent(ActiveRun.RunId, ActiveRun.CurrentFloor, ActiveRun.CurrentThemeId));
        }

        public void AbandonCurrentRun()
        {
            if (HasActiveRun)
            {
                var runId = ActiveRun.RunId;
                HasActiveRun = false;
                _eventBus.Publish(new RunAbandonedEvent(runId, ActiveRun));
                ActiveRun = null;
            }
        }

        public void CompleteCurrentRun(Dictionary<string, object> rewards)
        {
            if (HasActiveRun)
            {
                var runId = ActiveRun.RunId;
                HasActiveRun = false;
                _eventBus.Publish(new RunCompletedEvent(runId, ActiveRun, rewards));
                ActiveRun = null;
            }
        }
    }
}