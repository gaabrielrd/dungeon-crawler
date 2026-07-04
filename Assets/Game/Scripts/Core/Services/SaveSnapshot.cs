using System;

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
        }
    }

    [Serializable]
    public sealed class SaveProfileSnapshot
    {
        public int softCurrency;
        public int premiumCurrency;

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
    }
}
