using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "ShopTableDefinition",
        menuName = "DungeonCrawler/Data/Shops/Shop Table")]
    public sealed class ShopTableDefinition : GameDefinition
    {
        [Min(0)]
        [SerializeField] private int baseRerollCost;
        [SerializeField] private WeightedDefinitionEntry[] entries;

        public int BaseRerollCost => baseRerollCost;

        public WeightedDefinitionEntry[] Entries => entries;
    }
}
