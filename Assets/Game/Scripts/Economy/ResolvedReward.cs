using System;
using System.Collections.Generic;

namespace DungeonCrawler.Economy
{
    [Serializable]
    public sealed class ResolvedReward
    {
        public int softCurrency;
        public int xpReward;
        public bool isBossReward;
        public List<ResolvedItemReward> itemRewards = new();

        public ResolvedReward(int softCurrency, int xpReward, bool isBossReward, List<ResolvedItemReward> itemRewards)
        {
            this.softCurrency = softCurrency;
            this.xpReward = xpReward;
            this.isBossReward = isBossReward;
            this.itemRewards = itemRewards ?? new List<ResolvedItemReward>();
        }

        public int SoftCurrency => softCurrency;

        public int XpReward => xpReward;

        public bool IsBossReward => isBossReward;

        public IReadOnlyList<ResolvedItemReward> ItemRewards => itemRewards;

        public bool HasRewards => softCurrency > 0 || xpReward > 0 || itemRewards.Count > 0;
    }
}
