using System;
using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [Serializable]
    public struct WeightedDefinitionEntry
    {
        [SerializeField] private GameDefinition definition;
        [Min(0)]
        [SerializeField] private int weight;

        public GameDefinition Definition => definition;

        public int Weight => weight;

        public WeightedDefinitionEntry(GameDefinition definition, int weight)
        {
            this.definition = definition;
            this.weight = weight;
        }
    }
}
