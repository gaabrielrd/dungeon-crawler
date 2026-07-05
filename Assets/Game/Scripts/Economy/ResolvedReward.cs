using System;
using System.Collections.Generic;

namespace DungeonCrawler.Economy
{
    [Serializable]
    public sealed class ResolvedReward
    {
        public int softCurrency;
        public bool isBossReward;
        public List<ResolvedItemReward> itemRewards = new();

        public ResolvedReward(int softCurrency, bool isBossReward, List<ResolvedItemReward> itemRewards)
        {
            this.softCurrency = softCurrency;
            this.isBossReward = isBossReward;
            this.itemRewards = itemRewards ?? new List<ResolvedItemReward>();
        }

        public int SoftCurrency => softCurrency;

        public bool IsBossReward => isBossReward;

        public IReadOnlyList<ResolvedItemReward> ItemRewards => itemRewards;

        public bool HasRewards => softCurrency > 0 || itemRewards.Count > 0;
    }
}
