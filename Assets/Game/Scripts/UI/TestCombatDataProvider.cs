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
        [SerializeField] private SkillDefinition[] skillDefinitions;

        public HeroClassDefinition[] HeroDefinitions => heroDefinitions;
        public EnemyDefinition[] EnemyDefinitions => enemyDefinitions;
        public SkillDefinition[] SkillDefinitions => skillDefinitions;

        public HeroClassDefinition GetHero(string id)
        {
            for (var i = 0; i < heroDefinitions.Length; i++)
            {
                if (heroDefinitions[i] != null && heroDefinitions[i].Id == id)
                    return heroDefinitions[i];
            }
            return null;
        }

        public EnemyDefinition GetEnemy(string id)
        {
            for (var i = 0; i < enemyDefinitions.Length; i++)
            {
                if (enemyDefinitions[i] != null && enemyDefinitions[i].Id == id)
                    return enemyDefinitions[i];
            }
            return null;
        }

        public SkillDefinition GetSkill(string id)
        {
            for (var i = 0; i < skillDefinitions.Length; i++)
            {
                if (skillDefinitions[i] != null && skillDefinitions[i].Id == id)
                    return skillDefinitions[i];
            }
            return null;
        }
    }
}
