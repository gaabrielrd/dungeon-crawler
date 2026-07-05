using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class DungeonRunCombatIntegrationTests
    {
        private readonly List<ScriptableObject> _definitions = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var definition in _definitions)
            {
                if (definition != null)
                {
                    UnityEngine.Object.DestroyImmediate(definition);
                }
            }

            _definitions.Clear();
        }

        [Test]
        public async Task StartRun_GeneratesFloorOneAndMarksExploring()
        {
            var service = new DungeonRunService(new EventBus());

            var run = await service.StartRunAsync("seed");

            Assert.That(run.CurrentFloor, Is.EqualTo(1));
            Assert.That(run.CurrentFloorInfo, Is.Not.Null);
            Assert.That(run.CurrentFloorInfo.FloorNumber, Is.EqualTo(1));
            Assert.That(run.Status, Is.EqualTo(DungeonRunStatus.Exploring));
            Assert.That(run.CanAdvanceFloor, Is.False);
        }

        [Test]
        public async Task StartRun_WithTheme_GeneratesEncounterForCurrentFloor()
        {
            var service = new DungeonRunService(new EventBus())
            {
                CurrentThemeDefinition = CreateThemeWithEncounter()
            };

            var run = await service.StartRunAsync("seed");

            Assert.That(run.CurrentFloorInfo.Encounter, Is.Not.Null);
            Assert.That(run.CurrentFloorInfo.Encounter.DefinitionId, Is.EqualTo("enemy.test"));
        }

        [Test]
        public async Task StartCurrentFloorCombat_CreatesControllerAndMarksInCombat()
        {
            var service = new DungeonRunService(new EventBus());
            await service.StartRunAsync("seed");
            var formation = CreateFormation();

            var controller = service.StartCurrentFloorCombat(formation);

            Assert.That(controller, Is.Not.Null);
            Assert.That(service.CurrentCombatController, Is.SameAs(controller));
            Assert.That(service.ActiveRun.Status, Is.EqualTo(DungeonRunStatus.InCombat));
        }

        [Test]
        public async Task Victory_ResolvesFloorAndAllowsAdvance()
        {
            var service = new DungeonRunService(new EventBus());
            await service.StartRunAsync("seed");
            service.StartCurrentFloorCombat(CreateFormation());

            service.ResolveCurrentCombatResult(CombatState.Victory);

            Assert.That(service.ActiveRun.Status, Is.EqualTo(DungeonRunStatus.FloorResolved));
            Assert.That(service.ActiveRun.LastCombatResult, Is.EqualTo(CombatState.Victory));
            Assert.That(service.ActiveRun.CanAdvanceFloor, Is.True);

            service.AdvanceFloor();

            Assert.That(service.ActiveRun.CurrentFloor, Is.EqualTo(2));
            Assert.That(service.ActiveRun.Status, Is.EqualTo(DungeonRunStatus.Exploring));
            Assert.That(service.ActiveRun.CanAdvanceFloor, Is.False);
        }

        [Test]
        public async Task Defeat_FailsRunAndBlocksAdvance()
        {
            var service = new DungeonRunService(new EventBus());
            await service.StartRunAsync("seed");
            service.StartCurrentFloorCombat(CreateFormation());

            service.ResolveCurrentCombatResult(CombatState.Defeat);

            Assert.That(service.HasActiveRun, Is.True);
            Assert.That(service.ActiveRun.Status, Is.EqualTo(DungeonRunStatus.Failed));
            Assert.That(service.ActiveRun.LastCombatResult, Is.EqualTo(CombatState.Defeat));
            Assert.That(service.ActiveRun.CanAdvanceFloor, Is.False);
            Assert.That(() => service.AdvanceFloor(), Throws.InvalidOperationException);
        }

        [Test]
        public void CombatController_DoesNotReferenceDungeonTypes()
        {
            var controllerType = typeof(CombatController);
            var members = new List<MemberInfo>();
            members.AddRange(controllerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            members.AddRange(controllerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            members.AddRange(controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

            foreach (var member in members)
            {
                Assert.That(member.ToString(), Does.Not.Contain("DungeonCrawler.Dungeon"));
                Assert.That(member.ToString(), Does.Not.Contain("DungeonRun"));
            }
        }

        private DungeonThemeDefinition CreateThemeWithEncounter()
        {
            var enemy = ScriptableObject.CreateInstance<EnemyDefinition>();
            var table = ScriptableObject.CreateInstance<EncounterTableDefinition>();
            var theme = ScriptableObject.CreateInstance<DungeonThemeDefinition>();
            _definitions.Add(enemy);
            _definitions.Add(table);
            _definitions.Add(theme);

            SetPrivateField(typeof(GameDefinition), enemy, "id", "enemy.test");
            SetPrivateField(typeof(GameDefinition), enemy, "displayName", "Enemy Test");
            SetPrivateField(typeof(EnemyDefinition), enemy, "baseStats", CreateStats(10, 4, 1, 3));

            SetPrivateField(typeof(GameDefinition), table, "id", "encounter.table.test");
            SetPrivateField(typeof(GameDefinition), table, "displayName", "Test Encounters");
            SetPrivateField(typeof(EncounterTableDefinition), table, "entries", new[]
            {
                new WeightedDefinitionEntry(enemy, 1)
            });

            SetPrivateField(typeof(GameDefinition), theme, "id", "theme.test");
            SetPrivateField(typeof(GameDefinition), theme, "displayName", "Theme Test");
            SetPrivateField(typeof(DungeonThemeDefinition), theme, "commonEncounters", table);
            return theme;
        }

        private static CombatFormationState CreateFormation()
        {
            var formation = new CombatFormationState();
            formation.AddCombatant(new CombatantState("hero", "Hero", CombatSide.Player, 1, CreateStats(20, 8, 2, 5)));
            formation.AddCombatant(new CombatantState("enemy", "Enemy", CombatSide.Enemy, 1, CreateStats(12, 4, 1, 3)));
            return formation;
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
            var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on {declaringType.Name}.");
            field.SetValue(target, value);
        }
    }
}
