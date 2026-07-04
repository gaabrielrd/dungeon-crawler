using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "SkillDefinition",
        menuName = "DungeonCrawler/Data/Combat/Skill")]
    public sealed class SkillDefinition : GameDefinition
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private int[] validUserRanks;
        [SerializeField] private int[] validTargetRanks;
        [SerializeField] private SkillTargetType targetType;
        [SerializeField] private DamageType damageType;
        [SerializeField] private float damageMultiplier;
        [Min(0)]
        [SerializeField] private int cooldown;
        [SerializeField] private StatusEffectApplication[] effects;

        public Sprite Icon => icon;

        public int[] ValidUserRanks => validUserRanks;

        public int[] ValidTargetRanks => validTargetRanks;

        public SkillTargetType TargetType => targetType;

        public DamageType DamageType => damageType;

        public float DamageMultiplier => damageMultiplier;

        public int Cooldown => cooldown;

        public StatusEffectApplication[] Effects => effects;
    }
}
