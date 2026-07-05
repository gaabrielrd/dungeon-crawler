using System;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class CombatantTurnOrderTests
    {
        [Test]
        public void TieBreakingOrder_PlayerSidePriorityWhenSidesTie()
        {
            var formation = new CombatFormationState();
            formation.AddCombatant(
                new CombatantState("enemy", "Enemy", CombatSide.Enemy, 1, CreateStats(10, 5, 2, 10)));
            formation.AddCombatant(
                new CombatantState("hero", "Hero", CombatSide.Player, 1, CreateStats(10, 5, 2, 10)));

            var turnManager = new TurnManager(formation);
            turnManager.RebuildTurnOrder();

            Assert.That(turnManager.TurnOrder.Count, Is.EqualTo(2));
            Assert.That(turnManager.TurnOrder[0].Side, Is.EqualTo(CombatSide.Player));
            Assert.That(turnManager.TurnOrder[1].Side, Is.EqualTo(CombatSide.Enemy));
        }

        [Test]
        public void TurnOrderSortedBySpeedFirst()
        {
            var formation = new CombatFormationState();
            formation.AddCombatant(
                new CombatantState("slow_enemy", "Slow Enemy", CombatSide.Enemy, 1, CreateStats(10, 5, 2, 1)));
            formation.AddCombatant(
                new CombatantState("fast_hero", "Fast Hero", CombatSide.Player, 1, CreateStats(10, 5, 2, 10)));

            var turnManager = new TurnManager(formation);
            turnManager.RebuildTurnOrder();

            Assert.That(turnManager.TurnOrder.Count, Is.EqualTo(2));
            Assert.That(turnManager.TurnOrder[0].DisplayName, Is.EqualTo("fast_hero"));
            Assert.That(turnManager.TurnOrder[1].DisplayName, Is.EqualTo("slow_enemy"));
        }

        [Test]
        public void SpeedTieResolvesByRank()
        {
            var formation = new CombatFormationState();
            formation.AddCombatant(
                new CombatantState("enemy_front", "Enemy Front", CombatSide.Enemy, 1, CreateStats(10, 5, 2, 10)));
            formation.AddCombatant(
                new CombatantState("hero_back", "Hero Back", CombatSide.Player, 2, CreateStats(10, 5, 2, 10)));

            var turnManager = new TurnManager(formation);
            turnManager.RebuildTurnOrder();

            Assert.That(turnManager.TurnOrder.Count, Is.EqualTo(2));
            Assert.That(turnManager.TurnOrder[0].Rank, Is.EqualTo(1));
            Assert.That(turnManager.TurnOrder[1].Rank, Is.EqualTo(2));
        }

        [Test]
        public void DeadCombatantsDoNotReceiveTurns()
        {
            var formation = new CombatFormationState();
            formation.AddCombatant(
                new CombatantState("dead_hero", "Dead Hero", CombatSide.Player, 1, CreateStats(10, 5, 2, 10)));
            formation.AddCombatant(
                new CombatantState("alive_enemy", "Alive Enemy", CombatSide.Enemy, 1, CreateStats(10, 5, 2, 5)));

            var turnManager = new TurnManager(formation);

            var firstCombatant = turnManager.GetNextCombatant();
            Assert.That(firstCombatant, Is.Not.Null);
            Assert.That(firstCombatant.DisplayName, Is.EqualTo("alive_enemy"));
        }

        [Test]
        public void DeadCombatantsAreRemovedFromTurnOrderAfterRebuild()
        {
            var formation = new CombatFormationState();
            formation.AddCombatant(
                new CombatantState("hero", "Hero", CombatSide.Player, 1, CreateStats(10, 5, 2, 10)));
            formation.AddCombatant(
                new CombatantState("enemy", "Enemy", CombatSide.Enemy, 1, CreateStats(10, 5, 2, 5)));

            var turnManager = new TurnManager(formation);

            var enemy = formation.Combatants[1];
            enemy.CurrentHp = 0;

            turnManager.RebuildTurnOrder();

            Assert.That(turnManager.TurnOrder.Count, Is.EqualTo(1));
            Assert.That(turnManager.TurnOrder[0].DisplayName, Is.EqualTo("hero"));
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
