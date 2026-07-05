using System;
using System.Reflection;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;
using DungeonCrawler.Economy;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class RewardResolverTests
    {
        private RewardDefinition _rewardDefinition;
        private ItemDefinition _potion;
        private ItemDefinition _relic;

        [SetUp]
        public void SetUp()
        {
            _potion = CreateItem("item.test.potion", "Small Potion");
            _relic = CreateItem("item.test.relic", "Crypt Relic");
            _rewardDefinition = ScriptableObject.CreateInstance<RewardDefinition>();
            SetPrivateField(typeof(GameDefinition), _rewardDefinition, "id", "reward.test");
            SetPrivateField(typeof(GameDefinition), _rewardDefinition, "displayName", "Reward Test");
            SetPrivateField(typeof(RewardDefinition), _rewardDefinition, "commonGoldMin", 10);
            SetPrivateField(typeof(RewardDefinition), _rewardDefinition, "commonGoldMax", 10);
            SetPrivateField(typeof(RewardDefinition), _rewardDefinition, "bossGoldMin", 40);
            SetPrivateField(typeof(RewardDefinition), _rewardDefinition, "bossGoldMax", 40);
            SetPrivateField(typeof(RewardDefinition), _rewardDefinition, "itemRewards", new[]
            {
                new WeightedItemRewardEntry(_potion, 1),
                new WeightedItemRewardEntry(_relic, 3)
            });
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_rewardDefinition);
            UnityEngine.Object.DestroyImmediate(_potion);
            UnityEngine.Object.DestroyImmediate(_relic);
        }

        [Test]
        public void Resolve_CommonCombat_GrantsCommonGold()
        {
            var resolver = new RewardResolver();
            var context = new RewardContext("seed", 1, 100, FloorType.Combat, EncounterType.Common);

            var reward = resolver.Resolve(context, _rewardDefinition);

            Assert.That(reward.SoftCurrency, Is.EqualTo(10));
            Assert.That(reward.IsBossReward, Is.False);
        }

        [Test]
        public void Resolve_BossCombat_GrantsBossGold()
        {
            var resolver = new RewardResolver();
            var context = new RewardContext("seed", 5, 500, FloorType.Boss, EncounterType.Boss);

            var reward = resolver.Resolve(context, _rewardDefinition);

            Assert.That(reward.SoftCurrency, Is.EqualTo(40));
            Assert.That(reward.IsBossReward, Is.True);
        }

        [Test]
        public void Resolve_SameSeedAndTable_ReturnsSameItem()
        {
            var resolver = new RewardResolver();
            var context = new RewardContext("seed", 3, 300, FloorType.Combat, EncounterType.Common);

            var first = resolver.Resolve(context, _rewardDefinition);
            var second = resolver.Resolve(context, _rewardDefinition);

            Assert.That(first.ItemRewards.Count, Is.EqualTo(1));
            Assert.That(second.ItemRewards.Count, Is.EqualTo(1));
            Assert.That(second.ItemRewards[0].ItemId, Is.EqualTo(first.ItemRewards[0].ItemId));
            Assert.That(second.ItemRewards[0].Quantity, Is.EqualTo(first.ItemRewards[0].Quantity));
        }

        private static ItemDefinition CreateItem(string id, string displayName)
        {
            var item = ScriptableObject.CreateInstance<ItemDefinition>();
            SetPrivateField(typeof(GameDefinition), item, "id", id);
            SetPrivateField(typeof(GameDefinition), item, "displayName", displayName);
            return item;
        }

        private static void SetPrivateField(Type declaringType, object target, string fieldName, object value)
        {
            var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on {declaringType.Name}.");
            field.SetValue(target, value);
        }
    }
}
