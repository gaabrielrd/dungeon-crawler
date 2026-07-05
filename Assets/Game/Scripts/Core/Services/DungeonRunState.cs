using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;

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
        public string startedAtUtc;

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

        public string StartedAtUtc
        {
            get => startedAtUtc;
            set => startedAtUtc = value;
        }

        public static DungeonRunState CreateNew(string seed, List<CombatantState> party)
        {
            return new DungeonRunState
            {
                runId = Guid.NewGuid().ToString(),
                seed = seed ?? Guid.NewGuid().ToString(),
                currentFloor = 1,
                currentThemeId = "crypt",
                party = new List<CombatantState>(party),
                inventorySnapshot = new SaveProfileSnapshot(),
                visitedFloors = new HashSet<int> { 1 },
                startedAtUtc = DateTime.UtcNow.ToString("O")
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

            if (visitedFloors == null)
            {
                visitedFloors = new HashSet<int>();
            }
        }

        public void AdvanceFloor()
        {
            currentFloor++;
            visitedFloors.Add(currentFloor);

            if (currentFloor % 5 == 0 && currentFloor <= 20)
            {
                currentThemeId = GetNextThemeId();
            }
        }

        private string GetNextThemeId()
        {
            if (currentFloor == 5)
            {
                return "crypt.boss";
            }
            if (currentFloor == 10)
            {
                return "crypt";
            }
            if (currentFloor == 15)
            {
                return "crypt.boss";
            }
            if (currentFloor == 20)
            {
                return "crypt.transition";
            }

            return currentThemeId;
        }
    }
}