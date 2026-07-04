using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "EquipmentDefinition",
        menuName = "DungeonCrawler/Data/Items/Equipment")]
    public sealed class EquipmentDefinition : GameDefinition
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private Rarity rarity;
        [SerializeField] private CombatStats statBonuses;
        [SerializeField] private UpgradeDefinition[] allowedUpgrades;

        public Sprite Icon => icon;

        public EquipmentSlot Slot => slot;

        public Rarity Rarity => rarity;

        public CombatStats StatBonuses => statBonuses;

        public UpgradeDefinition[] AllowedUpgrades => allowedUpgrades;
    }
}
