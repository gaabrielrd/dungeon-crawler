using System;

namespace DungeonCrawler.Combat
{
    public static class CombatRank
    {
        public const int Front = 1;
        public const int Back = 4;
        public const int MaxCombatantsPerSide = 4;

        public static bool IsValid(int rank)
        {
            return rank >= Front && rank <= Back;
        }

        public static void Validate(int rank)
        {
            if (!IsValid(rank))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rank),
                    rank,
                    "Combat rank must be between 1 and 4.");
            }
        }
    }
}
