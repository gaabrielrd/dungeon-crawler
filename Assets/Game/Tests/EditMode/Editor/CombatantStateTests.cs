using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class CombatantStateTests
    {
        private readonly List<ScriptableObject> definitions = new List<ScriptableObject>();

        [TearDown]
        public void TearDown()
        {
            for (var index = 0; index < definitions.Count; index++)
            {
                UnityEngine.Object.DestroyImmediate(definitions[index]);
            }

            definitions.Clear();
        }

        [Test]
        public void FormationAcceptsPartyWithFourHeroes()
        {
            var formation = new CombatFormationState();

            for (var rank = CombatRank.Front; rank <= CombatRank.Back; rank++)
            {
                formation.AddCombatant(CombatantStateFactory.CreateHero(CreateHeroDefinition(rank), rank));
            }

            Assert.That(formation.CountSide(CombatSide.Player), Is.EqualTo(4));
            Assert.That(formation.CountSide(CombatSide.Enemy), Is.Zero);
        }

        [Test]
        public void FormationAcceptsEnemyGroupWithFourEnemies()
        {
            var formation = new CombatFormationState();

            for (var rank = CombatRank.Front; rank <= CombatRank.Back; rank++)
            {
                formation.AddCombatant(CombatantStateFactory.CreateEnemy(CreateEnemyDefinition(rank), rank));
            }

            Assert.That(formation.CountSide(CombatSide.Enemy), Is.EqualTo(4));
            Assert.That(formation.CountSide(CombatSide.Player), Is.Zero);
        }

        [TestCase(0)]
        [TestCase(5)]
        public void CombatantRejectsInvalidRank(int rank)
        {
            var definition = CreateHeroDefinition(1);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => CombatantStateFactory.CreateHero(definition, rank));
        }

        [Test]
        public void FormationRejectsMoreThanFourCombatantsOnSameSide()
        {
            var formation = new CombatFormationState();

            for (var rank = CombatRank.Front; rank <= CombatRank.Back; rank++)
            {
                formation.AddCombatant(CombatantStateFactory.CreateHero(CreateHeroDefinition(rank), rank));
            }

            var extraCombatant = CombatantStateFactory.CreateHero(CreateHeroDefinition(5), CombatRank.Front);

            Assert.Throws<InvalidOperationException>(() => formation.AddCombatant(extraCombatant));
        }

        [Test]
        public void FormationRejectsDuplicateRankOnSameSide()
        {
            var formation = new CombatFormationState();

            formation.AddCombatant(CombatantStateFactory.CreateHero(CreateHeroDefinition(1), CombatRank.Front));
            var duplicateRank = CombatantStateFactory.CreateHero(CreateHeroDefinition(2), CombatRank.Front);

            Assert.Throws<InvalidOperationException>(() => formation.AddCombatant(duplicateRank));
        }

        [Test]
        public void FactoryCopiesDefinitionStatsIntoRuntimeState()
        {
            var stats = CreateStats(maxHp: 32, attack: 7, defense: 5, speed: 11);
            var definition = CreateHeroDefinition(1, stats);

            var state = CombatantStateFactory.CreateHero(definition, CombatRank.Front);

            Assert.That(state.DefinitionId, Is.EqualTo("hero_1"));
            Assert.That(state.DisplayName, Is.EqualTo("Hero 1"));
            Assert.That(state.Side, Is.EqualTo(CombatSide.Player));
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

        [Test]
        public void IsAliveReflectsCurrentHp()
        {
            var state = CombatantStateFactory.CreateEnemy(CreateEnemyDefinition(1), CombatRank.Front);

            Assert.That(state.IsAlive, Is.True);

            state.CurrentHp = 0;

            Assert.That(state.IsAlive, Is.False);
        }

        private HeroClassDefinition CreateHeroDefinition(int index)
        {
            return CreateHeroDefinition(index, CreateStats(maxHp: 20 + index, attack: 5, defense: 2, speed: 3));
        }

        private HeroClassDefinition CreateHeroDefinition(int index, CombatStats stats)
        {
            var definition = ScriptableObject.CreateInstance<HeroClassDefinition>();
            definitions.Add(definition);

            SetGameDefinitionFields(definition, $"hero_{index}", $"Hero {index}");
            SetPrivateField(typeof(HeroClassDefinition), definition, "baseStats", stats);

            return definition;
        }

        private EnemyDefinition CreateEnemyDefinition(int index)
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definitions.Add(definition);

            SetGameDefinitionFields(definition, $"enemy_{index}", $"Enemy {index}");
            SetPrivateField(
                typeof(EnemyDefinition),
                definition,
                "baseStats",
                CreateStats(maxHp: 10 + index, attack: 4, defense: 1, speed: 2));

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

        private static void SetGameDefinitionFields(GameDefinition definition, string id, string displayName)
        {
            SetPrivateField(typeof(GameDefinition), definition, "id", id);
            SetPrivateField(typeof(GameDefinition), definition, "displayName", displayName);
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
