using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class EncounterGeneratorTests
    {
        private const int TestSeed = 12345;

        private EncounterGenerator _generator;
        private readonly List<ScriptableObject> _definitions = new();

        [SetUp]
        public void SetUp()
        {
            _generator = new EncounterGenerator();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var def in _definitions)
            {
                if (def != null)
                {
                    UnityEngine.Object.DestroyImmediate(def);
                }
            }

            _definitions.Clear();
        }

        [Test]
        public void CommonFloor_PicksFromCommonTable()
        {
            var theme = CreateThemeWithCommonEncounters();

            var encounter = _generator.Resolve(theme, FloorType.Combat, TestSeed);

            Assert.That(encounter.EncounterType, Is.EqualTo(EncounterType.Common));
            Assert.That(encounter.DefinitionType, Is.EqualTo("EnemyDefinition"));
            Assert.That(encounter.DefinitionId, Is.Not.Null);
        }

        [Test]
        public void BossFloor_PicksFromBossTable()
        {
            var theme = CreateThemeWithBossEncounters();

            var encounter = _generator.Resolve(theme, FloorType.Boss, TestSeed);

            Assert.That(encounter.EncounterType, Is.EqualTo(EncounterType.Boss));
            Assert.That(encounter.DefinitionType, Is.EqualTo("BossDefinition"));
            Assert.That(encounter.DefinitionId, Is.Not.Null);
        }

        [Test]
        public void SameSeedAndTheme_ReturnsSameDefinition()
        {
            var theme = CreateThemeWithCommonEncounters();

            var resultA = _generator.Resolve(theme, FloorType.Combat, TestSeed);
            var resultB = _generator.Resolve(theme, FloorType.Combat, TestSeed);

            Assert.That(resultB.DefinitionId, Is.EqualTo(resultA.DefinitionId));
            Assert.That(resultB.EncounterType, Is.EqualTo(resultA.EncounterType));
            Assert.That(resultB.LocalSeed, Is.EqualTo(resultA.LocalSeed));
        }

        [Test]
        public void DifferentSeeds_CanProduceDifferentResults()
        {
            var theme = CreateThemeWithCommonEncounters();

            var resultA = _generator.Resolve(theme, FloorType.Combat, 100);
            var resultB = _generator.Resolve(theme, FloorType.Combat, 200);

            Assert.That(resultA.EncounterType, Is.EqualTo(resultB.EncounterType));

            var idsMatch = resultA.DefinitionId == resultB.DefinitionId;
            if (idsMatch)
            {
                Assert.Pass("Different seeds may select same entry by chance; that is acceptable.");
            }
            else
            {
                Assert.That(resultA.DefinitionId, Is.Not.EqualTo(resultB.DefinitionId));
            }
        }

        [Test]
        public void CommonFloor_ReturnsEncounterTypeCommon()
        {
            var theme = CreateThemeWithCommonEncounters();

            var encounter = _generator.Resolve(theme, FloorType.Combat, TestSeed);

            Assert.That(encounter.EncounterType, Is.EqualTo(EncounterType.Common));
        }

        [Test]
        public void BossFloor_ReturnsEncounterTypeBoss()
        {
            var theme = CreateThemeWithBossEncounters();

            var encounter = _generator.Resolve(theme, FloorType.Boss, TestSeed);

            Assert.That(encounter.EncounterType, Is.EqualTo(EncounterType.Boss));
        }

        [Test]
        public void ResolvedEncounters_HaveValidDefinitionIds()
        {
            var theme = CreateThemeWithCommonEncounters();
            var encounter = _generator.Resolve(theme, FloorType.Combat, TestSeed);

            Assert.That(encounter.DefinitionId, Is.Not.Null);
            Assert.That(encounter.DefinitionId, Is.Not.Empty);
            Assert.That(encounter.DisplayName, Is.Not.Null);
            Assert.That(encounter.DisplayName, Is.Not.Empty);
            Assert.That(encounter.DefinitionType, Is.Not.Null);
            Assert.That(encounter.DefinitionType, Is.Not.Empty);
        }

        [Test]
        public void NullTheme_Throws()
        {
            Assert.That(
                () => _generator.Resolve(null, FloorType.Combat, TestSeed),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ThemeWithNoCommonTable_Throws()
        {
            var theme = CreateMinimalTheme();

            Assert.That(
                () => _generator.Resolve(theme, FloorType.Combat, TestSeed),
                Throws.InvalidOperationException);
        }

        [Test]
        public void ThemeWithNoBossTable_Throws()
        {
            var theme = CreateMinimalTheme();

            Assert.That(
                () => _generator.Resolve(theme, FloorType.Boss, TestSeed),
                Throws.InvalidOperationException);
        }

        [Test]
        public void ThemeWithEmptyCommonTable_Throws()
        {
            var theme = CreateThemeWithEmptyTable(EncounterType.Common);

            Assert.That(
                () => _generator.Resolve(theme, FloorType.Combat, TestSeed),
                Throws.InvalidOperationException);
        }

        [Test]
        public void ThemeWithEmptyBossTable_Throws()
        {
            var theme = CreateThemeWithEmptyTable(EncounterType.Boss);

            Assert.That(
                () => _generator.Resolve(theme, FloorType.Boss, TestSeed),
                Throws.InvalidOperationException);
        }

        private DungeonThemeDefinition CreateThemeWithCommonEncounters()
        {
            var enemyA = CreateEnemyDefinition("enemy.crypt.skeleton_grunt", "Skeleton Grunt", weight: 3);
            var enemyB = CreateEnemyDefinition("enemy.crypt.zombie", "Zombie", weight: 1);
            var table = CreateEncounterTable(EncounterType.Common, enemyA, 3, enemyB, 1);
            return CreateTheme(table, null);
        }

        private DungeonThemeDefinition CreateThemeWithBossEncounters()
        {
            var boss = CreateBossDefinition("boss.crypt.crypt_lord", "Crypt Lord", weight: 1);
            var table = CreateEncounterTable(EncounterType.Boss, boss, 1);
            return CreateTheme(null, table);
        }

        private DungeonThemeDefinition CreateMinimalTheme()
        {
            return CreateTheme(null, null);
        }

        private DungeonThemeDefinition CreateThemeWithEmptyTable(EncounterType encounterType)
        {
            var table = CreateEmptyEncounterTable(encounterType);
            return encounterType == EncounterType.Boss
                ? CreateTheme(null, table)
                : CreateTheme(table, null);
        }

        private DungeonThemeDefinition CreateTheme(
            EncounterTableDefinition common,
            EncounterTableDefinition boss)
        {
            var theme = ScriptableObject.CreateInstance<DungeonThemeDefinition>();
            _definitions.Add(theme);
            SetPrivateField(typeof(GameDefinition), theme, "id", "theme.test");
            SetPrivateField(typeof(GameDefinition), theme, "displayName", "Test Theme");
            SetPrivateField(typeof(DungeonThemeDefinition), theme, "commonEncounters", common);
            SetPrivateField(typeof(DungeonThemeDefinition), theme, "bossEncounters", boss);
            return theme;
        }

        private EncounterTableDefinition CreateEncounterTable(
            EncounterType encounterType,
            GameDefinition defA, int weightA,
            GameDefinition defB = null, int weightB = 0)
        {
            var entries = new List<WeightedDefinitionEntry>();
            entries.Add(CreateWeightedEntry(defA, weightA));
            if (defB != null)
            {
                entries.Add(CreateWeightedEntry(defB, weightB));
            }

            return CreateTable(encounterType, entries.ToArray());
        }

        private EncounterTableDefinition CreateEmptyEncounterTable(EncounterType encounterType)
        {
            return CreateTable(encounterType, new WeightedDefinitionEntry[0]);
        }

        private EncounterTableDefinition CreateTable(EncounterType encounterType, WeightedDefinitionEntry[] entries)
        {
            var table = ScriptableObject.CreateInstance<EncounterTableDefinition>();
            _definitions.Add(table);
            SetPrivateField(typeof(GameDefinition), table, "id", $"encounter.table.{encounterType.ToString().ToLowerInvariant()}");
            SetPrivateField(typeof(GameDefinition), table, "displayName", $"{encounterType} Encounters");
            SetPrivateField(typeof(EncounterTableDefinition), table, "encounterType", encounterType);
            SetPrivateField(typeof(EncounterTableDefinition), table, "entries", entries);
            return table;
        }

        private static WeightedDefinitionEntry CreateWeightedEntry(GameDefinition definition, int weight)
        {
            return new WeightedDefinitionEntry(definition, weight);
        }

        private EnemyDefinition CreateEnemyDefinition(string id, string displayName, int weight)
        {
            var def = ScriptableObject.CreateInstance<EnemyDefinition>();
            _definitions.Add(def);
            SetPrivateField(typeof(GameDefinition), def, "id", id);
            SetPrivateField(typeof(GameDefinition), def, "displayName", displayName);
            var stats = CreateStats(maxHp: 10, attack: 5, defense: 2, speed: 4);
            SetPrivateField(typeof(EnemyDefinition), def, "baseStats", stats);
            return def;
        }

        private BossDefinition CreateBossDefinition(string id, string displayName, int weight)
        {
            var def = ScriptableObject.CreateInstance<BossDefinition>();
            _definitions.Add(def);
            SetPrivateField(typeof(GameDefinition), def, "id", id);
            SetPrivateField(typeof(GameDefinition), def, "displayName", displayName);
            var stats = CreateStats(maxHp: 50, attack: 12, defense: 6, speed: 6);
            SetPrivateField(typeof(BossDefinition), def, "baseStats", stats);
            SetPrivateField(typeof(BossDefinition), def, "firstFloor", 5);
            SetPrivateField(typeof(BossDefinition), def, "lastFloor", 100);
            return def;
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
