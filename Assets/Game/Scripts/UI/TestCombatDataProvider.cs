using System;
using UnityEngine;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.UI
{
    [CreateAssetMenu(
        fileName = "TestCombatDataProvider",
        menuName = "DungeonCrawler/Test/Test Combat Data")]
    public sealed class TestCombatDataProvider : ScriptableObject
    {
        [SerializeField] private HeroClassDefinition[] heroDefinitions;
        [SerializeField] private EnemyDefinition[] enemyDefinitions;
        [SerializeField] private BossDefinition[] bossDefinitions;
        [SerializeField] private SkillDefinition[] skillDefinitions;

        public HeroClassDefinition[] HeroDefinitions => heroDefinitions ?? Array.Empty<HeroClassDefinition>();
        public EnemyDefinition[] EnemyDefinitions => enemyDefinitions ?? Array.Empty<EnemyDefinition>();
        public BossDefinition[] BossDefinitions => bossDefinitions ?? Array.Empty<BossDefinition>();
        public SkillDefinition[] SkillDefinitions => skillDefinitions ?? Array.Empty<SkillDefinition>();

        public HeroClassDefinition GetHero(string id)
        {
            var definitions = HeroDefinitions;
            for (var i = 0; i < definitions.Length; i++)
            {
                if (definitions[i] != null && definitions[i].Id == id)
                    return definitions[i];
            }
            return null;
        }

        public EnemyDefinition GetEnemy(string id)
        {
            var definitions = EnemyDefinitions;
            for (var i = 0; i < definitions.Length; i++)
            {
                if (definitions[i] != null && definitions[i].Id == id)
                    return definitions[i];
            }
            return null;
        }

        public BossDefinition GetBoss(string id)
        {
            var definitions = BossDefinitions;
            for (var i = 0; i < definitions.Length; i++)
            {
                if (definitions[i] != null && definitions[i].Id == id)
                    return definitions[i];
            }
            return null;
        }

        public SkillDefinition GetSkill(string id)
        {
            var definitions = SkillDefinitions;
            for (var i = 0; i < definitions.Length; i++)
            {
                if (definitions[i] != null && definitions[i].Id == id)
                    return definitions[i];
            }
            return null;
        }
    }
}
