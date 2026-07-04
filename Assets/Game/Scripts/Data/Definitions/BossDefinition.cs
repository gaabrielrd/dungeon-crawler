using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "BossDefinition",
        menuName = "DungeonCrawler/Data/Enemies/Boss")]
    public sealed class BossDefinition : GameDefinition
    {
        [SerializeField] private DungeonThemeDefinition theme;
        [SerializeField] private int firstFloor;
        [SerializeField] private int lastFloor;
        [SerializeField] private CombatStats baseStats;
        [TextArea]
        [SerializeField] private string mainMechanic;
        [SerializeField] private EnemyDefinition[] summonOptions;
        [SerializeField] private SkillDefinition dangerSkill;
        [SerializeField] private WeightedDefinitionEntry[] rewards;

        public DungeonThemeDefinition Theme => theme;

        public int FirstFloor => firstFloor;

        public int LastFloor => lastFloor;

        public CombatStats BaseStats => baseStats;

        public string MainMechanic => mainMechanic;

        public EnemyDefinition[] SummonOptions => summonOptions;

        public SkillDefinition DangerSkill => dangerSkill;

        public WeightedDefinitionEntry[] Rewards => rewards;
    }
}
