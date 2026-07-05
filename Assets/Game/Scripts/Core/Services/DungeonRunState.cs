using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Dungeon;
using DungeonCrawler.Economy;

namespace DungeonCrawler.Core.Services
{
    [Serializable]
    public sealed class DungeonRunState
    {
        public string runId;
        public string seed;
        public int currentFloor;
        public string currentThemeId;
        public List<CombatantState> party;
        public SaveProfileSnapshot inventorySnapshot;
        public HashSet<int> visitedFloors;
        public GeneratedFloor currentFloorInfo;
        public string startedAtUtc;
        public DungeonRunStatus status;
        public CombatState lastCombatResult;
        public bool canAdvanceFloor;
        public ResolvedReward lastResolvedReward;

        public string RunId
        {
            get => runId;
            set => runId = value;
        }

        public string Seed
        {
            get => seed;
            set => seed = value;
        }

        public int CurrentFloor
        {
            get => currentFloor;
            set => currentFloor = value;
        }

        public string CurrentThemeId
        {
            get => currentThemeId;
            set => currentThemeId = value;
        }

        public List<CombatantState> Party
        {
            get => party;
            set => party = value;
        }

        public SaveProfileSnapshot InventorySnapshot
        {
            get => inventorySnapshot;
            set => inventorySnapshot = value;
        }

        public HashSet<int> VisitedFloors
        {
            get => visitedFloors;
            set => visitedFloors = value;
        }

        public GeneratedFloor CurrentFloorInfo
        {
            get => currentFloorInfo;
            set => currentFloorInfo = value;
        }

        public string StartedAtUtc
        {
            get => startedAtUtc;
            set => startedAtUtc = value;
        }

        public DungeonRunStatus Status
        {
            get => status;
            set => status = value;
        }

        public CombatState LastCombatResult
        {
            get => lastCombatResult;
            set => lastCombatResult = value;
        }

        public bool CanAdvanceFloor
        {
            get => canAdvanceFloor;
            set => canAdvanceFloor = value;
        }

        public ResolvedReward LastResolvedReward
        {
            get => lastResolvedReward;
            set => lastResolvedReward = value;
        }

        public static DungeonRunState CreateNew(string seed, List<CombatantState> party)
        {
            return new DungeonRunState
            {
                runId = Guid.NewGuid().ToString(),
                seed = seed ?? Guid.NewGuid().ToString(),
                currentFloor = 1,
                currentThemeId = "crypt",
                party = party == null ? new List<CombatantState>() : new List<CombatantState>(party),
                inventorySnapshot = new SaveProfileSnapshot(),
                visitedFloors = new HashSet<int>(),
                startedAtUtc = DateTime.UtcNow.ToString("O"),
                status = DungeonRunStatus.NotStarted,
                lastCombatResult = CombatState.Initializing,
                canAdvanceFloor = false,
                lastResolvedReward = null
            };
        }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(runId))
            {
                runId = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(seed))
            {
                seed = Guid.NewGuid().ToString();
            }

            if (party == null)
            {
                party = new List<CombatantState>();
            }

            if (inventorySnapshot == null)
            {
                inventorySnapshot = new SaveProfileSnapshot();
            }

            inventorySnapshot.Normalize();

            if (visitedFloors == null)
            {
                visitedFloors = new HashSet<int>();
            }

            if (status == default && currentFloor > 0)
            {
                status = DungeonRunStatus.Exploring;
            }
        }

        public void AdvanceFloor()
        {
            currentFloor++;
            visitedFloors.Add(currentFloor);
            lastResolvedReward = null;
        }

        public void ApplyFloorGeneration(GeneratedFloor floor)
        {
            currentFloorInfo = floor;
            currentThemeId = floor.ThemeId;
        }
    }
}
