using System;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;

namespace DungeonCrawler.Economy
{
    public readonly struct RewardContext
    {
        public RewardContext(
            string runSeed,
            int floorNumber,
            int localSeed,
            FloorType floorType,
            EncounterType encounterType)
        {
            RunSeed = runSeed ?? string.Empty;
            FloorNumber = floorNumber;
            LocalSeed = localSeed;
            FloorType = floorType;
            EncounterType = encounterType;
        }

        public string RunSeed { get; }

        public int FloorNumber { get; }

        public int LocalSeed { get; }

        public FloorType FloorType { get; }

        public EncounterType EncounterType { get; }

        public bool IsBoss => FloorType == DungeonCrawler.Dungeon.FloorType.Boss
            || EncounterType == DungeonCrawler.Data.Definitions.EncounterType.Boss;

        public int CreateDeterministicSeed(string salt)
        {
            unchecked
            {
                var hash = 2166136261;
                hash = AddString(hash, RunSeed);
                hash = AddInt(hash, FloorNumber);
                hash = AddInt(hash, LocalSeed);
                hash = AddInt(hash, (int)FloorType);
                hash = AddInt(hash, (int)EncounterType);
                hash = AddString(hash, salt ?? string.Empty);
                return (int)(hash & 0x7fffffff);
            }
        }

        private static uint AddString(uint hash, string value)
        {
            for (var index = 0; index < value.Length; index++)
            {
                hash ^= value[index];
                hash *= 16777619;
            }

            return hash;
        }

        private static uint AddInt(uint hash, int value)
        {
            hash ^= (uint)value;
            hash *= 16777619;
            return hash;
        }
    }
}
