using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using UnityEngine;

namespace DungeonCrawler.Data.State
{
    [Serializable]
    public sealed class HeroState
    {
        [SerializeField] private string classId;
        [SerializeField] private string heroName;
        [SerializeField] private int level;
        [SerializeField] private int currentXp;
        [SerializeField] private int maxHp;
        [SerializeField] private int currentHp;
        [SerializeField] private int attack;
        [SerializeField] private int defense;
        [SerializeField] private int speed;
        [SerializeField] private List<string> learnedSkillIds;
        [SerializeField] private Rarity rarity;
        [SerializeField] private bool isInParty;
        [SerializeField] private int partyRank;
        [SerializeField] private string weaponId;
        [SerializeField] private string armorId;
        [SerializeField] private string accessoryId;

        public HeroState(
            HeroClassDefinition definition,
            string heroName,
            Rarity rarity)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            classId = definition.Id;
            this.heroName = heroName ?? definition.DisplayName;
            level = 1;
            currentXp = 0;
            this.rarity = rarity;

            var stats = definition.BaseStats;
            maxHp = stats.MaxHp;
            currentHp = stats.MaxHp;
            attack = HeroProgressionService.GetAverageDamage(level);
            defense = stats.Defense;
            speed = stats.Speed;

            learnedSkillIds = new List<string>();
            if (definition.StartingSkills != null)
            {
                foreach (var skill in definition.StartingSkills)
                {
                    if (skill != null && !string.IsNullOrEmpty(skill.Id))
                        learnedSkillIds.Add(skill.Id);
                }
            }

            isInParty = false;
            partyRank = 0;
            weaponId = null;
            armorId = null;
            accessoryId = null;
        }

        public string ClassId => classId;
        public string HeroName => heroName;
        public int Level => level;
        public int CurrentXp => currentXp;
        public int XpToNextLevel => HeroProgressionService.GetXpToNextLevel(level);
        public int BaseAverageDamage => HeroProgressionService.GetAverageDamage(level);
        public bool IsMaxLevel => level >= HeroProgressionService.MaxLevel;
        public int MaxHp => maxHp;
        public int CurrentHp { get => currentHp; set => currentHp = Math.Max(0, value); }
        public int Attack => attack;
        public int Defense => defense;
        public int Speed => speed;
        public IReadOnlyList<string> LearnedSkillIds => learnedSkillIds;
        public Rarity Rarity => rarity;
        public bool IsInParty { get => isInParty; set => isInParty = value; }
        public int PartyRank { get => partyRank; set => partyRank = value; }
        public string WeaponId { get => weaponId; set => weaponId = value; }
        public string ArmorId { get => armorId; set => armorId = value; }
        public string AccessoryId { get => accessoryId; set => accessoryId = value; }

        public bool IsAlive => currentHp > 0;
        public bool IsDead => !IsAlive;

        public void TakeDamage(int amount)
        {
            currentHp = Math.Max(0, currentHp - Math.Max(0, amount));
        }

        public void Heal(int amount)
        {
            currentHp = Math.Min(maxHp, currentHp + Math.Max(0, amount));
        }

        public void AddXp(int amount)
        {
            currentXp = Math.Max(0, currentXp + amount);
        }

        public void LevelUp()
        {
            if (level >= HeroProgressionService.MaxLevel)
                return;

            level++;
            var hpIncrease = HeroProgressionService.GetAverageDamage(level);
            maxHp += hpIncrease;
            currentHp = maxHp;
            attack = HeroProgressionService.GetAverageDamage(level);
        }

        public bool LearnSkill(string skillId)
        {
            if (string.IsNullOrEmpty(skillId))
                return false;
            if (learnedSkillIds.Contains(skillId))
                return false;
            learnedSkillIds.Add(skillId);
            return true;
        }

        public bool HasSkill(string skillId)
        {
            return learnedSkillIds.Contains(skillId);
        }

        public CombatantState CreateCombatantState(int rank)
        {
            return new CombatantState(
                classId,
                heroName,
                CombatSide.Player,
                rank,
                new CombatStats(maxHp, attack, defense, speed));
        }

        public void SyncAfterCombat(CombatantState combatState)
        {
            if (combatState == null)
                return;
            currentHp = combatState.CurrentHp;
        }
    }
}
