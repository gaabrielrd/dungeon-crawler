using System;
using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [Serializable]
    public struct WeightedItemRewardEntry
    {
        [SerializeField] private ItemDefinition item;
        [Min(0)]
        [SerializeField] private int weight;
        [Min(1)]
        [SerializeField] private int quantityMin;
        [Min(1)]
        [SerializeField] private int quantityMax;

        public ItemDefinition Item => item;

        public int Weight => weight;

        public int QuantityMin => quantityMin <= 0 ? 1 : quantityMin;

        public int QuantityMax => quantityMax < QuantityMin ? QuantityMin : quantityMax;

        public WeightedItemRewardEntry(ItemDefinition item, int weight, int quantityMin = 1, int quantityMax = 1)
        {
            this.item = item;
            this.weight = weight;
            this.quantityMin = quantityMin;
            this.quantityMax = quantityMax;
        }
    }
}
