using System;

namespace DungeonCrawler.Dungeon
{
    public sealed class FloorGenerator
    {
        private static readonly string[] ThemeCycle =
        {
            "crypt",
            "catacombs",
            "infernal",
            "void"
        };

        public GeneratedFloor GenerateFloor(string runSeed, int floorNumber, string currentThemeId)
        {
            if (runSeed == null) throw new ArgumentNullException(nameof(runSeed));
            if (currentThemeId == null) throw new ArgumentNullException(nameof(currentThemeId));

            var localSeed = ComputeLocalSeed(runSeed, floorNumber, currentThemeId);

            var isThemeTransition = floorNumber > 0 && floorNumber % 20 == 0;
            var isBoss = floorNumber > 0 && floorNumber % 5 == 0;
            var hasRestingSite = floorNumber > 0 && floorNumber % 10 == 0;

            var floorType = isBoss ? FloorType.Boss : FloorType.Combat;

            string nextThemeId = null;
            var themeId = currentThemeId;

            if (isThemeTransition)
            {
                nextThemeId = ResolveNextThemeId(currentThemeId);
                themeId = nextThemeId;
            }

            return new GeneratedFloor(
                floorNumber,
                floorType,
                hasRestingSite,
                isThemeTransition,
                themeId,
                nextThemeId,
                localSeed);
        }

        private static int ComputeLocalSeed(string runSeed, int floorNumber, string themeId)
        {
            var runHash = StableHash(runSeed);
            var themeHash = StableHash(themeId);
            return HashCode.Combine(runHash, floorNumber, themeHash);
        }

        private static int StableHash(string value)
        {
            unchecked
            {
                var hash = 17;
                foreach (var c in value)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        private static string ResolveNextThemeId(string currentThemeId)
        {
            for (var i = 0; i < ThemeCycle.Length; i++)
            {
                if (string.Equals(ThemeCycle[i], currentThemeId, StringComparison.OrdinalIgnoreCase))
                {
                    return ThemeCycle[(i + 1) % ThemeCycle.Length];
                }
            }

            return ThemeCycle[0];
        }
    }
}
