using System;
using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [Serializable]
    public struct CombatStats
    {
        [SerializeField] private int maxHp;
        [SerializeField] private int attack;
        [SerializeField] private int magic;
        [SerializeField] private int defense;
        [SerializeField] private int speed;

        public int MaxHp => maxHp;

        public int Attack => attack;

        public int Magic => magic;

        public int Defense => defense;

        public int Speed => speed;
    }
}
