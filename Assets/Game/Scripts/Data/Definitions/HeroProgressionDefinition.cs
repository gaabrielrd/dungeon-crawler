using System;
using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "HeroProgressionDefinition",
        menuName = "DungeonCrawler/Data/Heroes/Hero Progression")]
    public sealed class HeroProgressionDefinition : GameDefinition
    {
        [SerializeField] private LevelEntry[] levels;

        public LevelEntry[] Levels => levels;

        public int MaxLevel
        {
            get
            {
                if (levels == null || levels.Length == 0)
                    return 0;
                return levels[levels.Length - 1].Level;
            }
        }

        public LevelEntry GetLevelEntry(int level)
        {
            if (levels == null)
                return null;
            for (var i = 0; i < levels.Length; i++)
            {
                if (levels[i].Level == level)
                    return levels[i];
            }
            return null;
        }

        public int GetAverageDamage(int level)
        {
            var entry = GetLevelEntry(level);
            return entry != null ? entry.BaseAverageDamage : 0;
        }

        public int GetDamageRangeMin(int level)
        {
            var entry = GetLevelEntry(level);
            return entry != null ? entry.DamageRangeMin : 0;
        }

        public int GetDamageRangeMax(int level)
        {
            var entry = GetLevelEntry(level);
            return entry != null ? entry.DamageRangeMax : 0;
        }

        public int GetXpToNextLevel(int level)
        {
            var entry = GetLevelEntry(level);
            return entry != null ? entry.XpToNextLevel : 0;
        }

        [Serializable]
        public sealed class LevelEntry
        {
            [SerializeField] private int level;
            [SerializeField] private int baseAverageDamage;
            [SerializeField] private int damageRangeMin;
            [SerializeField] private int damageRangeMax;
            [SerializeField] private int xpToNextLevel;

            public LevelEntry(
                int level,
                int baseAverageDamage,
                int damageRangeMin,
                int damageRangeMax,
                int xpToNextLevel)
            {
                this.level = level;
                this.baseAverageDamage = baseAverageDamage;
                this.damageRangeMin = damageRangeMin;
                this.damageRangeMax = damageRangeMax;
                this.xpToNextLevel = xpToNextLevel;
            }

            public int Level => level;
            public int BaseAverageDamage => baseAverageDamage;
            public int DamageRangeMin => damageRangeMin;
            public int DamageRangeMax => damageRangeMax;
            public int XpToNextLevel => xpToNextLevel;
        }
    }
}
