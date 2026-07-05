using System;
using System.Collections.Generic;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Data.State;

namespace DungeonCrawler.Combat
{
    public static class HeroProgressionService
    {
        public const int MaxLevel = 10;

        private static readonly int[] FibonacciDamage =
            { 0, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233 };

        private static readonly int[] XpToNext =
            { 0, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377 };

        private static readonly int[] DefaultMinRange =
            { 0, 2, 4, 6, 10, 17, 27, 44, 71, 115, 186 };

        private static readonly int[] DefaultMaxRange =
            { 0, 4, 6, 10, 16, 25, 41, 66, 107, 173, 280 };

        public static int GetAverageDamage(int level)
        {
            if (level < 1) return FibonacciDamage[1];
            if (level > MaxLevel) return FibonacciDamage[MaxLevel];
            return FibonacciDamage[level];
        }

        public static int GetDamageRangeMin(int level)
        {
            if (level < 1) return DefaultMinRange[1];
            if (level > MaxLevel) return DefaultMinRange[MaxLevel];
            return DefaultMinRange[level];
        }

        public static int GetDamageRangeMax(int level)
        {
            if (level < 1) return DefaultMaxRange[1];
            if (level > MaxLevel) return DefaultMaxRange[MaxLevel];
            return DefaultMaxRange[level];
        }

        public static int GetXpToNextLevel(int level)
        {
            if (level < 1) return XpToNext[1];
            if (level > MaxLevel) return XpToNext[MaxLevel];
            return XpToNext[level];
        }

        public static int GetTotalXpForLevel(int level)
        {
            if (level <= 1) return 0;
            var total = 0;
            for (var i = 1; i < level && i <= MaxLevel; i++)
            {
                total += XpToNext[i];
            }
            return total;
        }

        public static float GetSkillLevelMultiplier(int skillLevel)
        {
            if (skillLevel < 1) return 1f;
            if (skillLevel > 3) skillLevel = 3;
            return FibonacciDamage[skillLevel] / 3f;
        }

        public static bool TryLevelUp(HeroState hero, HeroProgressionDefinition def = null)
        {
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));

            if (hero.Level >= MaxLevel)
                return false;

            var xpRequired = def != null
                ? def.GetXpToNextLevel(hero.Level)
                : GetXpToNextLevel(hero.Level);

            if (hero.CurrentXp < xpRequired)
                return false;

            hero.AddXp(-xpRequired);
            hero.LevelUp();

            return true;
        }

        public static List<SkillDefinition> GetAvailableSkills(
            HeroClassDefinition classDef,
            HeroState hero)
        {
            if (classDef == null)
                throw new ArgumentNullException(nameof(classDef));
            if (hero == null)
                throw new ArgumentNullException(nameof(hero));

            var available = new List<SkillDefinition>();
            var entries = classDef.SkillsByLevel;
            if (entries == null || entries.Length == 0)
                return available;

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry == null || entry.Skill == null)
                    continue;
                if (entry.Level <= hero.Level && !hero.HasSkill(entry.Skill.Id))
                {
                    available.Add(entry.Skill);
                }
            }

            return available;
        }

        public static bool IsMaxLevel(int level)
        {
            return level >= MaxLevel;
        }
    }
}
