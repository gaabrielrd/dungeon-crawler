using System;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class BasicAttackTests
    {
        [Test]
        public void HeroBasicAttackDamagesEnemy()
        {
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 4, defense: 2);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 8, defense: 1),
                enemy));

            controller.StartCombat();
            var result = controller.ExecuteBasicAttack(enemy);

            Assert.That(result.Damage, Is.EqualTo(6));
            Assert.That(enemy.CurrentHp, Is.EqualTo(14));
        }

        [Test]
        public void EnemyBasicAttackDamagesHero()
        {
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 1, maxHp: 20, attack: 4, defense: 3);
            var controller = CreateController(CreateFormation(
                hero,
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 10, maxHp: 20, attack: 9, defense: 1)));

            controller.StartCombat();
            var result = controller.ExecuteBasicAttack(hero);

            Assert.That(result.Damage, Is.EqualTo(6));
            Assert.That(hero.CurrentHp, Is.EqualTo(14));
        }

        [Test]
        public void BasicAttackDealsAtLeastOneDamage()
        {
            var attacker = CreateCombatant("attacker", CombatSide.Player, rank: 1, speed: 1, maxHp: 20, attack: 3, defense: 1);
            var target = CreateCombatant("target", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 3, defense: 10);
            var resolver = new DamageResolver();

            var result = resolver.ResolveBasicAttack(attacker, target);

            Assert.That(result.Damage, Is.EqualTo(1));
            Assert.That(target.CurrentHp, Is.EqualTo(19));
        }

        [Test]
        public void BasicAttackDoesNotReduceHpBelowZero()
        {
            var attacker = CreateCombatant("attacker", CombatSide.Player, rank: 1, speed: 1, maxHp: 20, attack: 50, defense: 1);
            var target = CreateCombatant("target", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 10, attack: 3, defense: 0);
            var resolver = new DamageResolver();

            var result = resolver.ResolveBasicAttack(attacker, target);

            Assert.That(result.Damage, Is.EqualTo(50));
            Assert.That(result.TargetHpBefore, Is.EqualTo(10));
            Assert.That(result.TargetHpAfter, Is.Zero);
            Assert.That(target.CurrentHp, Is.Zero);
        }

        [Test]
        public void BasicAttackRejectsDeadTarget()
        {
            var target = CreateCombatant("target", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 3, defense: 1);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 5, defense: 1),
                target));

            controller.StartCombat();
            target.CurrentHp = 0;

            Assert.Throws<InvalidOperationException>(() => controller.ExecuteBasicAttack(target));
            Assert.That(controller.State, Is.EqualTo(CombatState.PlayerTurn));
            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("hero"));
        }

        [Test]
        public void BasicAttackRejectsAllyTarget()
        {
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2, speed: 2, maxHp: 20, attack: 3, defense: 1);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 5, defense: 1),
                ally,
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 3, defense: 1)));

            controller.StartCombat();

            Assert.Throws<InvalidOperationException>(() => controller.ExecuteBasicAttack(ally));
            Assert.That(controller.State, Is.EqualTo(CombatState.PlayerTurn));
            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("hero"));
        }

        [Test]
        public void BasicAttackRejectsDeadAttacker()
        {
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 5, defense: 1);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 3, defense: 1);
            var controller = CreateController(CreateFormation(hero, enemy));

            controller.StartCombat();
            hero.CurrentHp = 0;

            Assert.Throws<InvalidOperationException>(() => controller.ExecuteBasicAttack(enemy));
            Assert.That(controller.State, Is.EqualTo(CombatState.PlayerTurn));
            Assert.That(controller.CurrentCombatant, Is.SameAs(hero));
        }

        [Test]
        public void BasicAttackPublishesDamageResolvedEvent()
        {
            var eventBus = new EventBus();
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 4, defense: 2);
            var controller = new CombatController(
                CreateFormation(
                    CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 8, defense: 1),
                    enemy),
                eventBus);
            DamageResult eventResult = default;
            var eventCount = 0;

            eventBus.Subscribe<DamageResolvedEvent>(damageResolved =>
            {
                eventCount++;
                eventResult = damageResolved.Result;
            });

            controller.StartCombat();
            controller.ExecuteBasicAttack(enemy);

            Assert.That(eventCount, Is.EqualTo(1));
            Assert.That(eventResult.Attacker.DefinitionId, Is.EqualTo("hero"));
            Assert.That(eventResult.Target, Is.SameAs(enemy));
            Assert.That(eventResult.Damage, Is.EqualTo(6));
            Assert.That(eventResult.TargetHpBefore, Is.EqualTo(20));
            Assert.That(eventResult.TargetHpAfter, Is.EqualTo(14));
        }

        [Test]
        public void ValidBasicAttackAdvancesTurn()
        {
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 20, attack: 4, defense: 2);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 8, defense: 1),
                enemy));

            controller.StartCombat();
            controller.ExecuteBasicAttack(enemy);

            Assert.That(controller.State, Is.EqualTo(CombatState.EnemyTurn));
            Assert.That(controller.CurrentCombatant, Is.SameAs(enemy));
        }

        [Test]
        public void BasicAttackCanEndCombatWithVictory()
        {
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1, maxHp: 5, attack: 4, defense: 0);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10, maxHp: 20, attack: 8, defense: 1),
                enemy));

            controller.StartCombat();
            controller.ExecuteBasicAttack(enemy);

            Assert.That(enemy.CurrentHp, Is.Zero);
            Assert.That(controller.State, Is.EqualTo(CombatState.Victory));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void BasicAttackCanEndCombatWithDefeat()
        {
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 1, maxHp: 5, attack: 4, defense: 0);
            var controller = CreateController(CreateFormation(
                hero,
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 10, maxHp: 20, attack: 8, defense: 1)));

            controller.StartCombat();
            controller.ExecuteBasicAttack(hero);

            Assert.That(hero.CurrentHp, Is.Zero);
            Assert.That(controller.State, Is.EqualTo(CombatState.Defeat));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        private static CombatController CreateController(CombatFormationState formation)
        {
            return new CombatController(formation, new EventBus());
        }

        private static CombatFormationState CreateFormation(params CombatantState[] combatants)
        {
            var formation = new CombatFormationState();

            for (var index = 0; index < combatants.Length; index++)
            {
                formation.AddCombatant(combatants[index]);
            }

            return formation;
        }

        private static CombatantState CreateCombatant(
            string id,
            CombatSide side,
            int rank,
            int speed,
            int maxHp,
            int attack,
            int defense)
        {
            return new CombatantState(
                id,
                id,
                side,
                rank,
                CreateStats(maxHp, attack, defense, speed));
        }

        private static CombatStats CreateStats(int maxHp, int attack, int defense, int speed)
        {
            object boxedStats = new CombatStats();
            SetPrivateField("maxHp", boxedStats, maxHp);
            SetPrivateField("attack", boxedStats, attack);
            SetPrivateField("defense", boxedStats, defense);
            SetPrivateField("speed", boxedStats, speed);

            return (CombatStats)boxedStats;
        }

        private static void SetPrivateField(string fieldName, object target, object value)
        {
            var field = typeof(CombatStats).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on CombatStats.");

            field.SetValue(target, value);
        }
    }
}
