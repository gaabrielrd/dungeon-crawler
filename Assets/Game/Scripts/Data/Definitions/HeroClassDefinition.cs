using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "HeroClassDefinition",
        menuName = "DungeonCrawler/Data/Heroes/Hero Class")]
    public sealed class HeroClassDefinition : GameDefinition
    {
        [SerializeField] private string role;
        [SerializeField] private int[] preferredRanks;
        [SerializeField] private CombatStats baseStats;
        [SerializeField] private SkillDefinition[] startingSkills;
        [SerializeField] private SkillDefinition[] unlockableSkills;

        public string Role => role;

        public int[] PreferredRanks => preferredRanks;

        public CombatStats BaseStats => baseStats;

        public SkillDefinition[] StartingSkills => startingSkills;

        public SkillDefinition[] UnlockableSkills => unlockableSkills;
    }
}
