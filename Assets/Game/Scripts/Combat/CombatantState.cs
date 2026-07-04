using System;
using DungeonCrawler.Data.Definitions;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    [Serializable]
    public sealed class CombatantState
    {
        [SerializeField] private string definitionId;
        [SerializeField] private string displayName;
        [SerializeField] private CombatSide side;
        [SerializeField] private int rank;
        [SerializeField] private int maxHp;
        [SerializeField] private int currentHp;
        [SerializeField] private int speed;
        [SerializeField] private int attack;
        [SerializeField] private int defense;

        public CombatantState(
            string definitionId,
            string displayName,
            CombatSide side,
            int rank,
            CombatStats stats)
        {
            CombatRank.Validate(rank);

            this.definitionId = definitionId ?? string.Empty;
            this.displayName = displayName ?? string.Empty;
            this.side = side;
            this.rank = rank;
            maxHp = Math.Max(0, stats.MaxHp);
            currentHp = maxHp;
            speed = stats.Speed;
            attack = stats.Attack;
            defense = stats.Defense;
        }

        public string DefinitionId => definitionId;

        public string DisplayName => displayName;

        public CombatSide Side => side;

        public int Rank => rank;

        public int MaxHp => maxHp;

        public int CurrentHp
        {
            get => currentHp;
            set => currentHp = Clamp(value, 0, maxHp);
        }

        public int Speed => speed;

        public int Attack => attack;

        public int Defense => defense;

        public bool IsAlive => currentHp > 0;

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }
    }
}
