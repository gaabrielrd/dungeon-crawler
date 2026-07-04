using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "EnemyDefinition",
        menuName = "DungeonCrawler/Data/Enemies/Enemy")]
    public sealed class EnemyDefinition : GameDefinition
    {
        [SerializeField] private DungeonThemeDefinition theme;
        [SerializeField] private string role;
        [SerializeField] private int[] occupiedRanks;
        [SerializeField] private CombatStats baseStats;
        [SerializeField] private SkillDefinition[] skills;
        [Min(0)]
        [SerializeField] private int rewardWeight;

        public DungeonThemeDefinition Theme => theme;

        public string Role => role;

        public int[] OccupiedRanks => occupiedRanks;

        public CombatStats BaseStats => baseStats;

        public SkillDefinition[] Skills => skills;

        public int RewardWeight => rewardWeight;
    }
}
