using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.UI;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class CombatPrototypeTurnSelectionUtilityTests
    {
        [Test]
        public void BasicAttackTargetsExcludeAlliesAndDeadEnemies()
        {
            var attacker = CreateCombatant("hero", CombatSide.Player, rank: 1, maxHp: 20);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2, maxHp: 20);
            var livingEnemy = CreateCombatant("enemy_alive", CombatSide.Enemy, rank: 1, maxHp: 20);
            var deadEnemy = CreateCombatant("enemy_dead", CombatSide.Enemy, rank: 2, maxHp: 20);
            deadEnemy.CurrentHp = 0;

            var targets = CombatPrototypeTurnSelectionUtility.GetValidBasicAttackTargets(
                attacker,
                new List<CombatantState> { attacker, ally, livingEnemy, deadEnemy });

            Assert.That(targets, Is.EquivalentTo(new[] { livingEnemy }));
        }

        [Test]
        public void ChooseRandomTargetReturnsNullForEmptyList()
        {
            var selected = CombatPrototypeTurnSelectionUtility.ChooseRandomTarget(
                new List<CombatantState>(),
                new System.Random(42));

            Assert.That(selected, Is.Null);
        }

        [Test]
        public void ChooseRandomTargetReturnsDeterministicSelectionWithSeed()
        {
            var candidates = new List<CombatantState>
            {
                CreateCombatant("hero_1", CombatSide.Player, rank: 1, maxHp: 20),
                CreateCombatant("hero_2", CombatSide.Player, rank: 2, maxHp: 20),
                CreateCombatant("hero_3", CombatSide.Player, rank: 3, maxHp: 20)
            };

            var selected = CombatPrototypeTurnSelectionUtility.ChooseRandomTarget(candidates, new System.Random(7));

            Assert.That(selected, Is.SameAs(candidates[1]));
        }

        [Test]
        public void IsValidBasicAttackTargetRejectsNullAndSameSide()
        {
            var attacker = CreateCombatant("hero", CombatSide.Player, rank: 1, maxHp: 20);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2, maxHp: 20);

            Assert.That(CombatPrototypeTurnSelectionUtility.IsValidBasicAttackTarget(attacker, null), Is.False);
            Assert.That(CombatPrototypeTurnSelectionUtility.IsValidBasicAttackTarget(attacker, ally), Is.False);
        }

        private static CombatantState CreateCombatant(string id, CombatSide side, int rank, int maxHp)
        {
            return new CombatantState(
                id,
                id,
                side,
                rank,
                CreateStats(maxHp, attack: 8, defense: 2, speed: 8));
        }

        private static CombatStats CreateStats(int maxHp, int attack, int defense, int speed)
        {
            object boxedStats = new CombatStats();
            SetPrivateField(boxedStats, "maxHp", maxHp);
            SetPrivateField(boxedStats, "attack", attack);
            SetPrivateField(boxedStats, "defense", defense);
            SetPrivateField(boxedStats, "speed", speed);
            return (CombatStats)boxedStats;
        }

        private static void SetPrivateField(object target, string fieldName, int value)
        {
            var field = typeof(CombatStats).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on CombatStats.");
            field.SetValue(target, value);
        }
    }
}
