using System;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Dungeon
{
    [Serializable]
    public sealed class ResolvedEncounter
    {
        public string DefinitionId { get; }
        public string DisplayName { get; }
        public EncounterType EncounterType { get; }
        public int LocalSeed { get; }
        public string DefinitionType { get; }

        public ResolvedEncounter(
            string definitionId,
            string displayName,
            EncounterType encounterType,
            int localSeed,
            string definitionType)
        {
            DefinitionId = definitionId ?? throw new ArgumentNullException(nameof(definitionId));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            EncounterType = encounterType;
            LocalSeed = localSeed;
            DefinitionType = definitionType ?? throw new ArgumentNullException(nameof(definitionType));
        }
    }
}
