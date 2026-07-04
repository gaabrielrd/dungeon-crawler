using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "EncounterTableDefinition",
        menuName = "DungeonCrawler/Data/Dungeon/Encounter Table")]
    public sealed class EncounterTableDefinition : GameDefinition
    {
        [SerializeField] private EncounterType encounterType;
        [SerializeField] private WeightedDefinitionEntry[] entries;

        public EncounterType EncounterType => encounterType;

        public WeightedDefinitionEntry[] Entries => entries;
    }
}
