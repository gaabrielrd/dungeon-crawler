using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "UpgradeDefinition",
        menuName = "DungeonCrawler/Data/Progression/Upgrade")]
    public sealed class UpgradeDefinition : GameDefinition
    {
        [SerializeField] private UpgradeType upgradeType;
        [Min(0)]
        [SerializeField] private int baseCost;
        [SerializeField] private float increment;
        [SerializeField] private GameDefinition targetDefinition;
        [SerializeField] private string targetDefinitionId;

        public UpgradeType UpgradeType => upgradeType;

        public int BaseCost => baseCost;

        public float Increment => increment;

        public GameDefinition TargetDefinition => targetDefinition;

        public string TargetDefinitionId => targetDefinitionId;
    }
}
