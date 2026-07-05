using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "RewardDefinition",
        menuName = "DungeonCrawler/Data/Economy/Reward")]
    public sealed class RewardDefinition : GameDefinition
    {
        [Min(0)]
        [SerializeField] private int commonGoldMin = 10;
        [Min(0)]
        [SerializeField] private int commonGoldMax = 15;
        [Min(0)]
        [SerializeField] private int bossGoldMin = 35;
        [Min(0)]
        [SerializeField] private int bossGoldMax = 45;
        [SerializeField] private WeightedItemRewardEntry[] itemRewards;

        public int CommonGoldMin => commonGoldMin;

        public int CommonGoldMax => commonGoldMax < commonGoldMin ? commonGoldMin : commonGoldMax;

        public int BossGoldMin => bossGoldMin;

        public int BossGoldMax => bossGoldMax < bossGoldMin ? bossGoldMin : bossGoldMax;

        public WeightedItemRewardEntry[] ItemRewards => itemRewards;
    }
}
