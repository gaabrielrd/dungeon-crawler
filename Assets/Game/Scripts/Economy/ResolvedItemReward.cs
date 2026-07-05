using System;

namespace DungeonCrawler.Economy
{
    [Serializable]
    public sealed class ResolvedItemReward
    {
        public string itemId;
        public string displayName;
        public int quantity;

        public ResolvedItemReward(string itemId, string displayName, int quantity)
        {
            this.itemId = itemId;
            this.displayName = displayName;
            this.quantity = quantity;
        }

        public string ItemId => itemId;

        public string DisplayName => displayName;

        public int Quantity => quantity;
    }
}
