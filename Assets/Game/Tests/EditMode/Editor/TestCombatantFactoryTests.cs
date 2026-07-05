using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class TestCombatantFactoryCreationTests
    {
        [Test]
        public void CombatantFactoryCreatesHeroesWithPlayerSide()
        {
            var heroDef = CreateHeroDefinition(1);
            var state = CombatantStateFactory.CreateHero(heroDef, CombatRank.Front);
            Assert.That(state.Side, Is.EqualTo(CombatSide.Player));
        }

        [Test]
        public void CombatantFactoryCreatesEnemiesWithEnemySide()
        {
            var enemyDef = CreateEnemyDefinition(1);
            var state = CombatantStateFactory.CreateEnemy(enemyDef, CombatRank.Front);
            Assert.That(state.Side, Is.EqualTo(CombatSide.Enemy));
        }

        [Test]
        public void CombatantFactoryCreatesBossesWithEnemySide()
        {
            var bossDef = CreateBossDefinition(1);
            var state = CombatantStateFactory.CreateBoss(bossDef, CombatRank.Front);
            Assert.That(state.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(state.DefinitionId, Is.EqualTo("boss_1"));
            Assert.That(state.MaxHp, Is.EqualTo(bossDef.BaseStats.MaxHp));
        }

        [Test]
        public void CombatantFactoryCreatesWithCorrectStatsFromDefinitions()
        {
            var stats = CreateStats(maxHp: 32, attack: 7, defense: 5, speed: 11);
            var definition = CreateHeroDefinition(1, stats);
            var state = CombatantStateFactory.CreateHero(definition, CombatRank.Front);

            Assert.That(state.DefinitionId, Is.EqualTo("hero_1"));
            Assert.That(state.DisplayName, Is.EqualTo("Hero 1"));
            Assert.That(state.Rank, Is.EqualTo(CombatRank.Front));
            Assert.That(state.MaxHp, Is.EqualTo(32));
            Assert.That(state.CurrentHp, Is.EqualTo(32));
            Assert.That(state.Attack, Is.EqualTo(7));
            Assert.That(state.Defense, Is.EqualTo(5));
            Assert.That(state.Speed, Is.EqualTo(11));
        }

        [Test]
        public void MutatingRuntimeHpDoesNotModifyDefinitionStats()
        {
            var stats = CreateStats(maxHp: 42, attack: 8, defense: 3, speed: 6);
            var definition = CreateHeroDefinition(1, stats);
            var state = CombatantStateFactory.CreateHero(definition, CombatRank.Front);

            state.CurrentHp = 5;

            Assert.That(state.CurrentHp, Is.EqualTo(5));
            Assert.That(definition.BaseStats.MaxHp, Is.EqualTo(42));
        }

        private HeroClassDefinition CreateHeroDefinition(int index, CombatStats stats)
        {
            var definition = ScriptableObject.CreateInstance<HeroClassDefinition>();
            var allDefinitions = new List<ScriptableObject>();

            SetPrivateField(typeof(GameDefinition), definition, "id", $"hero_{index}");
            SetPrivateField(typeof(GameDefinition), definition, "displayName", $"Hero {index}");
            SetPrivateField(typeof(HeroClassDefinition), definition, "baseStats", stats);

            return definition;
        }

        private HeroClassDefinition CreateHeroDefinition(int index)
        {
            return CreateHeroDefinition(index, CreateStats(maxHp: 20 + index, attack: 5, defense: 2, speed: 3));
        }

        private EnemyDefinition CreateEnemyDefinition(int index)
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            var allDefinitions = new List<ScriptableObject>();

            SetPrivateField(typeof(GameDefinition), definition, "id", $"enemy_{index}");
            SetPrivateField(typeof(GameDefinition), definition, "displayName", $"Enemy {index}");
            SetPrivateField(typeof(EnemyDefinition), definition, "baseStats",
                CreateStats(maxHp: 10 + index, attack: 4, defense: 1, speed: 2));

            return definition;
        }

        private BossDefinition CreateBossDefinition(int index)
        {
            var definition = ScriptableObject.CreateInstance<BossDefinition>();

            SetPrivateField(typeof(GameDefinition), definition, "id", $"boss_{index}");
            SetPrivateField(typeof(GameDefinition), definition, "displayName", $"Boss {index}");
            SetPrivateField(typeof(BossDefinition), definition, "baseStats",
                CreateStats(maxHp: 40 + index, attack: 9, defense: 4, speed: 3));

            return definition;
        }

        private static CombatStats CreateStats(int maxHp, int attack, int defense, int speed)
        {
            object boxedStats = new CombatStats();
            SetPrivateField(typeof(CombatStats), boxedStats, "maxHp", maxHp);
            SetPrivateField(typeof(CombatStats), boxedStats, "attack", attack);
            SetPrivateField(typeof(CombatStats), boxedStats, "defense", defense);
            SetPrivateField(typeof(CombatStats), boxedStats, "speed", speed);

            return (CombatStats)boxedStats;
        }

        private static void SetPrivateField(Type declaringType, object target, string fieldName, object value)
        {
            var field = declaringType.GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on {declaringType.Name}.");

            field.SetValue(target, value);
        }
    }
}
