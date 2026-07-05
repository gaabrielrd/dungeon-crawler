using System;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Dungeon
{
    public sealed class EncounterGenerator
    {
        public ResolvedEncounter Resolve(DungeonThemeDefinition theme, FloorType floorType, int localSeed)
        {
            if (theme == null) throw new ArgumentNullException(nameof(theme));

            var table = SelectTable(theme, floorType);
            if (table == null)
            {
                throw new InvalidOperationException(
                    $"No encounter table found for floor type {floorType} on theme '{theme.Id}'.");
            }

            var entries = table.Entries;
            if (entries == null || entries.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Encounter table '{table.Id}' has no entries.");
            }

            var selected = PickWeighted(entries, localSeed);
            var definitionType = selected.GetType().Name;
            var encounterType = MapToEncounterType(floorType);

            return new ResolvedEncounter(
                selected.Id,
                selected.DisplayName,
                encounterType,
                localSeed,
                definitionType);
        }

        private static EncounterTableDefinition SelectTable(DungeonThemeDefinition theme, FloorType floorType)
        {
            return floorType switch
            {
                FloorType.Boss => theme.BossEncounters,
                _ => theme.CommonEncounters
            };
        }

        private static EncounterType MapToEncounterType(FloorType floorType)
        {
            return floorType switch
            {
                FloorType.Boss => EncounterType.Boss,
                _ => EncounterType.Common
            };
        }

        private static GameDefinition PickWeighted(WeightedDefinitionEntry[] entries, int localSeed)
        {
            var totalWeight = 0;
            foreach (var entry in entries)
            {
                totalWeight += entry.Weight;
            }

            if (totalWeight <= 0)
            {
                throw new InvalidOperationException("Total weight must be greater than zero.");
            }

            var roll = new System.Random(localSeed).Next(0, totalWeight);
            var accumulated = 0;

            foreach (var entry in entries)
            {
                accumulated += entry.Weight;
                if (roll < accumulated)
                {
                    return entry.Definition;
                }
            }

            return entries[entries.Length - 1].Definition;
        }
    }
}
