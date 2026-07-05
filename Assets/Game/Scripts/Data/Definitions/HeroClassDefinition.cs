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
        [SerializeField] private int[] validRanks;
        [SerializeField] private CombatStats baseStats;
        [SerializeField] private Rarity baseRarity;
        [SerializeField] private SkillDefinition[] startingSkills;
        [SerializeField] private SkillByLevelEntry[] skillsByLevel;
        [SerializeField] private SkillDefinition[] unlockableSkills;
        [SerializeField] private string[] namePool;
        [SerializeField] private string unlockCondition;
        [SerializeField] private string[] allowedWeaponTypes;
        [SerializeField] private string[] allowedArmorTypes;
        [SerializeField] private string[] allowedAccessoryTypes;

        [System.Serializable]
        public sealed class SkillByLevelEntry
        {
            [SerializeField] private int level;
            [SerializeField] private SkillDefinition skill;

            public int Level => level;
            public SkillDefinition Skill => skill;
        }

        public string Role => role;

        public int[] PreferredRanks => preferredRanks;

        public int[] ValidRanks => validRanks;

        public CombatStats BaseStats => baseStats;

        public Rarity BaseRarity => baseRarity;

        public SkillDefinition[] StartingSkills => startingSkills;

        public SkillByLevelEntry[] SkillsByLevel => skillsByLevel;

        public SkillDefinition[] UnlockableSkills => unlockableSkills;

        public string[] NamePool => namePool;

        public string UnlockCondition => unlockCondition;

        public string[] AllowedWeaponTypes => allowedWeaponTypes;

        public string[] AllowedArmorTypes => allowedArmorTypes;

        public string[] AllowedAccessoryTypes => allowedAccessoryTypes;
    }
}
