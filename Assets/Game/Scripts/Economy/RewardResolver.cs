using System;
using System.Collections.Generic;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Economy
{
    public sealed class RewardResolver
    {
        private const int DefaultCommonGoldMin = 10;
        private const int DefaultCommonGoldMax = 15;
        private const int DefaultBossGoldMin = 35;
        private const int DefaultBossGoldMax = 45;

        private const int XpBasePerFloor = 5;
        private const int XpPerFloorMultiplier = 3;
        private const float XpBossMultiplier = 1.5f;

        public ResolvedReward Resolve(RewardContext context, RewardDefinition definition)
        {
            var gold = ResolveGold(context, definition);
            var xp = ResolveXp(context);
            var items = ResolveItems(context, definition);

            return new ResolvedReward(gold, xp, context.IsBoss, items);
        }

        private static int ResolveXp(RewardContext context)
        {
            var xp = XpBasePerFloor + (context.FloorNumber * XpPerFloorMultiplier);
            if (context.IsBoss)
                xp = (int)(xp * XpBossMultiplier);
            return Math.Max(1, xp);
        }

        private static int ResolveGold(RewardContext context, RewardDefinition definition)
        {
            var min = context.IsBoss
                ? definition?.BossGoldMin ?? DefaultBossGoldMin
                : definition?.CommonGoldMin ?? DefaultCommonGoldMin;
            var max = context.IsBoss
                ? definition?.BossGoldMax ?? DefaultBossGoldMax
                : definition?.CommonGoldMax ?? DefaultCommonGoldMax;

            if (max < min)
            {
                max = min;
            }

            var random = new Random(context.CreateDeterministicSeed("gold"));
            return random.Next(min, max + 1);
        }

        private static List<ResolvedItemReward> ResolveItems(RewardContext context, RewardDefinition definition)
        {
            var rewards = new List<ResolvedItemReward>();
            var entries = definition?.ItemRewards;
            if (entries == null || entries.Length == 0)
            {
                return rewards;
            }

            var totalWeight = 0;
            for (var index = 0; index < entries.Length; index++)
            {
                if (entries[index].Item != null && entries[index].Weight > 0)
                {
                    totalWeight += entries[index].Weight;
                }
            }

            if (totalWeight <= 0)
            {
                return rewards;
            }

            var itemRandom = new Random(context.CreateDeterministicSeed("item"));
            var roll = itemRandom.Next(0, totalWeight);
            var accumulated = 0;

            for (var index = 0; index < entries.Length; index++)
            {
                var entry = entries[index];
                if (entry.Item == null || entry.Weight <= 0)
                {
                    continue;
                }

                accumulated += entry.Weight;
                if (roll >= accumulated)
                {
                    continue;
                }

                var quantityRandom = new Random(context.CreateDeterministicSeed($"item_quantity_{entry.Item.Id}"));
                var quantity = quantityRandom.Next(entry.QuantityMin, entry.QuantityMax + 1);
                rewards.Add(new ResolvedItemReward(entry.Item.Id, entry.Item.DisplayName, quantity));
                return rewards;
            }

            return rewards;
        }
    }
}
