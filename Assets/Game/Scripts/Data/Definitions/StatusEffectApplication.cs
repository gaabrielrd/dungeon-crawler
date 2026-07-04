using System;
using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [Serializable]
    public struct StatusEffectApplication
    {
        [SerializeField] private StatusEffectDefinition statusEffect;
        [Range(0f, 1f)]
        [SerializeField] private float chance;
        [Min(1)]
        [SerializeField] private int stacks;

        public StatusEffectDefinition StatusEffect => statusEffect;

        public float Chance => chance;

        public int Stacks => stacks;
    }
}
