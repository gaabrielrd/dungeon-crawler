using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "ItemDefinition",
        menuName = "DungeonCrawler/Data/Items/Item")]
    public sealed class ItemDefinition : GameDefinition
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemType itemType;
        [Min(1)]
        [SerializeField] private int maxStack = 1;
        [Min(0)]
        [SerializeField] private int value;
        [SerializeField] private ItemUseTarget useTarget;

        public Sprite Icon => icon;

        public ItemType ItemType => itemType;

        public int MaxStack => maxStack;

        public int Value => value;

        public ItemUseTarget UseTarget => useTarget;
    }
}
