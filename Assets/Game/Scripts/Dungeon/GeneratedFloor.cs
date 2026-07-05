using System;

namespace DungeonCrawler.Dungeon
{
    [Serializable]
    public sealed class GeneratedFloor
    {
        public int FloorNumber { get; }
        public FloorType PrimaryType { get; }
        public bool HasRestingSite { get; }
        public bool IsThemeTransition { get; }
        public string ThemeId { get; }
        public string NextThemeId { get; }
        public int LocalSeed { get; }
        public ResolvedEncounter Encounter { get; }

        public GeneratedFloor(
            int floorNumber,
            FloorType primaryType,
            bool hasRestingSite,
            bool isThemeTransition,
            string themeId,
            string nextThemeId,
            int localSeed,
            ResolvedEncounter encounter = null)
        {
            FloorNumber = floorNumber;
            PrimaryType = primaryType;
            HasRestingSite = hasRestingSite;
            IsThemeTransition = isThemeTransition;
            ThemeId = themeId ?? throw new ArgumentNullException(nameof(themeId));
            NextThemeId = nextThemeId;
            LocalSeed = localSeed;
            Encounter = encounter;
        }

        public GeneratedFloor WithEncounter(ResolvedEncounter encounter)
        {
            if (encounter == null) throw new ArgumentNullException(nameof(encounter));
            return new GeneratedFloor(FloorNumber, PrimaryType, HasRestingSite, IsThemeTransition,
                ThemeId, NextThemeId, LocalSeed, encounter);
        }
    }
}
