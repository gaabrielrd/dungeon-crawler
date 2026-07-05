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

        public CombatStats(int maxHp, int attack, int defense, int speed)
        {
            this.maxHp = maxHp;
            this.attack = attack;
            this.magic = 0;
            this.defense = defense;
            this.speed = speed;
        }

        public CombatStats(int maxHp, int attack, int magic, int defense, int speed)
        {
            this.maxHp = maxHp;
            this.attack = attack;
            this.magic = magic;
            this.defense = defense;
            this.speed = speed;
        }

        public int MaxHp => maxHp;

        public int Attack => attack;

        public int Magic => magic;

        public int Defense => defense;

        public int Speed => speed;
    }
}
