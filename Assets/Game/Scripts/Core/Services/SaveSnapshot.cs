using System;
using System.Collections.Generic;

namespace DungeonCrawler.Core.Services
{
    [Serializable]
    public sealed class SaveSnapshot
    {
        public int saveVersion = 1;
        public string playerId;
        public string lastUpdatedUtc;
        public SaveProfileSnapshot profile = new();

        public int SaveVersion
        {
            get => saveVersion;
            set => saveVersion = value;
        }

        public string PlayerId
        {
            get => playerId;
            set => playerId = value;
        }

        public string LastUpdatedUtc
        {
            get => lastUpdatedUtc;
            set => lastUpdatedUtc = value;
        }

        public SaveProfileSnapshot Profile
        {
            get => profile;
            set => profile = value;
        }

        public static SaveSnapshot CreateNew(string playerId)
        {
            return new SaveSnapshot
            {
                saveVersion = 1,
                playerId = playerId,
                lastUpdatedUtc = DateTime.UtcNow.ToString("O"),
                profile = new SaveProfileSnapshot()
            };
        }

        public void Normalize()
        {
            if (saveVersion <= 0)
            {
                saveVersion = 1;
            }

            if (profile == null)
            {
                profile = new SaveProfileSnapshot();
            }

            profile.Normalize();
        }
    }

    [Serializable]
    public sealed class SaveProfileSnapshot
    {
        public int softCurrency;
        public int premiumCurrency;
        public List<ItemStackSnapshot> itemStacks = new();

        public int SoftCurrency
        {
            get => softCurrency;
            set => softCurrency = value;
        }

        public int PremiumCurrency
        {
            get => premiumCurrency;
            set => premiumCurrency = value;
        }

        public List<ItemStackSnapshot> ItemStacks
        {
            get => itemStacks;
            set => itemStacks = value;
        }

        public void Normalize()
        {
            if (itemStacks == null)
            {
                itemStacks = new List<ItemStackSnapshot>();
            }
        }

        public void AddItem(string itemId, int quantity)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0)
            {
                return;
            }

            Normalize();

            for (var index = 0; index < itemStacks.Count; index++)
            {
                var stack = itemStacks[index];
                if (stack != null && stack.ItemId == itemId)
                {
                    stack.Quantity += quantity;
                    return;
                }
            }

            itemStacks.Add(new ItemStackSnapshot(itemId, quantity));
        }
    }

    [Serializable]
    public sealed class ItemStackSnapshot
    {
        public string itemId;
        public int quantity;

        public ItemStackSnapshot(string itemId, int quantity)
        {
            this.itemId = itemId;
            this.quantity = quantity;
        }

        public string ItemId
        {
            get => itemId;
            set => itemId = value;
        }

        public int Quantity
        {
            get => quantity;
            set => quantity = value;
        }
    }
}
