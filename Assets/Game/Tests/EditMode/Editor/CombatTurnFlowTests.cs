using System;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class CombatTurnFlowTests
    {
        [Test]
        public void StartCombatAcceptsValidFormation()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10),
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 5)));

            controller.StartCombat();

            Assert.That(controller.State, Is.EqualTo(CombatState.PlayerTurn));
            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("hero"));
        }

        [Test]
        public void FirstTurnUsesSpeedOrder()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("slow_hero", CombatSide.Player, rank: 1, speed: 2),
                CreateCombatant("fast_enemy", CombatSide.Enemy, rank: 1, speed: 12)));

            controller.StartCombat();

            Assert.That(controller.State, Is.EqualTo(CombatState.EnemyTurn));
            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("fast_enemy"));
        }

        [Test]
        public void SpeedTieUsesPlayerThenRank()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 10),
                CreateCombatant("back_hero", CombatSide.Player, rank: 2, speed: 10),
                CreateCombatant("front_hero", CombatSide.Player, rank: 1, speed: 10)));

            controller.StartCombat();

            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("front_hero"));
        }

        [Test]
        public void SpeedTieUsesPlayerPriorityWhenSidesTie()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 10),
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10)));

            controller.StartCombat();

            Assert.That(controller.CurrentCombatant.Side, Is.EqualTo(CombatSide.Player));
        }

        [Test]
        public void TurnOrderSortedBySpeedFirst()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("slow_hero", CombatSide.Player, rank: 1, speed: 2),
                CreateCombatant("fast_enemy", CombatSide.Enemy, rank: 1, speed: 12)));

            controller.StartCombat();

            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("fast_enemy"));
        }

        [Test]
        public void MultipleDeadCombatantsDoNotReceiveTurns()
        {
            var skippedHero1 = CreateCombatant("skipped_hero_1", CombatSide.Player, rank: 1, speed: 5);
            var skippedHero2 = CreateCombatant("skipped_hero_2", CombatSide.Player, rank: 2, speed: 5);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1);
            var controller = CreateController(CreateFormation(
                skippedHero1,
                skippedHero2,
                enemy));

            controller.StartCombat();
            skippedHero1.CurrentHp = 0;
            skippedHero2.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.Defeat));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void DeadCombatantsAreRemovedFromTurnOrderAfterRebuild()
        {
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 5);
            var controller = CreateController(CreateFormation(hero, enemy));

            controller.StartCombat();
            enemy.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.Victory));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void CompleteCurrentTurnAdvancesToNextLivingCombatant()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10),
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 5)));

            controller.StartCombat();
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.EnemyTurn));
            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("enemy"));
        }

        [Test]
        public void DeadCombatantsDoNotReceiveTurns()
        {
            var skippedHero = CreateCombatant("skipped_hero", CombatSide.Player, rank: 2, speed: 5);
            var controller = CreateController(CreateFormation(
                CreateCombatant("active_hero", CombatSide.Player, rank: 1, speed: 10),
                skippedHero,
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1)));

            controller.StartCombat();
            skippedHero.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.EnemyTurn));
            Assert.That(controller.CurrentCombatant.DefinitionId, Is.EqualTo("enemy"));
        }

        [Test]
        public void CombatEndsWithVictoryWhenAllEnemiesAreDead()
        {
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10),
                enemy));

            controller.StartCombat();
            enemy.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.Victory));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void CombatEndsWithDefeatWhenAllPlayersAreDead()
        {
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10);
            var controller = CreateController(CreateFormation(
                hero,
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1)));

            controller.StartCombat();
            hero.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.Defeat));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void CombatControllerPublishesLifecycleEvents()
        {
            var eventBus = new EventBus();
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1);
            var controller = new CombatController(
                CreateFormation(CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10), enemy),
                eventBus);
            var combatStartedCount = 0;
            var stateChangedCount = 0;
            var turnStartedCount = 0;
            var turnEndedCount = 0;
            var combatEndedCount = 0;
            var combatVictoryCount = 0;
            var combatDefeatCount = 0;
            CombatState resultState = CombatState.Initializing;

            eventBus.Subscribe<CombatStartedEvent>(_ => combatStartedCount++);
            eventBus.Subscribe<CombatStateChangedEvent>(_ => stateChangedCount++);
            eventBus.Subscribe<TurnStartedEvent>(_ => turnStartedCount++);
            eventBus.Subscribe<TurnEndedEvent>(_ => turnEndedCount++);
            eventBus.Subscribe<CombatVictoryEvent>(_ => combatVictoryCount++);
            eventBus.Subscribe<CombatDefeatEvent>(_ => combatDefeatCount++);
            eventBus.Subscribe<CombatEndedEvent>(combatEnded =>
            {
                combatEndedCount++;
                resultState = combatEnded.ResultState;
            });

            controller.StartCombat();
            enemy.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(combatStartedCount, Is.EqualTo(1));
            Assert.That(stateChangedCount, Is.EqualTo(3));
            Assert.That(turnStartedCount, Is.EqualTo(1));
            Assert.That(turnEndedCount, Is.EqualTo(1));
            Assert.That(combatVictoryCount, Is.EqualTo(1));
            Assert.That(combatDefeatCount, Is.Zero);
            Assert.That(combatEndedCount, Is.EqualTo(1));
            Assert.That(resultState, Is.EqualTo(CombatState.Victory));
        }

        [Test]
        public void CombatControllerPublishesDefeatEvent()
        {
            var eventBus = new EventBus();
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10);
            var controller = new CombatController(
                CreateFormation(hero, CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1)),
                eventBus);
            var combatVictoryCount = 0;
            var combatDefeatCount = 0;
            var combatEndedCount = 0;
            CombatState resultState = CombatState.Initializing;

            eventBus.Subscribe<CombatVictoryEvent>(_ => combatVictoryCount++);
            eventBus.Subscribe<CombatDefeatEvent>(_ => combatDefeatCount++);
            eventBus.Subscribe<CombatEndedEvent>(combatEnded =>
            {
                combatEndedCount++;
                resultState = combatEnded.ResultState;
            });

            controller.StartCombat();
            hero.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(combatVictoryCount, Is.Zero);
            Assert.That(combatDefeatCount, Is.EqualTo(1));
            Assert.That(combatEndedCount, Is.EqualTo(1));
            Assert.That(resultState, Is.EqualTo(CombatState.Defeat));
        }

        [Test]
        public void CombatRejectsActionsAfterVictory()
        {
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1);
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10),
                enemy));

            controller.StartCombat();
            enemy.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.Victory));
            Assert.Throws<InvalidOperationException>(() => controller.ExecuteBasicAttack(enemy));
            Assert.Throws<InvalidOperationException>(() => controller.CompleteCurrentTurn());
            Assert.That(controller.State, Is.EqualTo(CombatState.Victory));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void CombatRejectsActionsAfterDefeat()
        {
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1);
            var controller = CreateController(CreateFormation(hero, enemy));

            controller.StartCombat();
            hero.CurrentHp = 0;
            controller.CompleteCurrentTurn();

            Assert.That(controller.State, Is.EqualTo(CombatState.Defeat));
            Assert.Throws<InvalidOperationException>(() => controller.ExecuteBasicAttack(hero));
            Assert.Throws<InvalidOperationException>(() => controller.CompleteCurrentTurn());
            Assert.That(controller.State, Is.EqualTo(CombatState.Defeat));
            Assert.That(controller.CurrentCombatant, Is.Null);
        }

        [Test]
        public void StartCombatRejectsFormationWithoutPlayers()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("enemy", CombatSide.Enemy, rank: 1, speed: 1)));

            Assert.Throws<InvalidOperationException>(() => controller.StartCombat());
        }

        [Test]
        public void StartCombatRejectsFormationWithoutEnemies()
        {
            var controller = CreateController(CreateFormation(
                CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 1)));

            Assert.Throws<InvalidOperationException>(() => controller.StartCombat());
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

        private static CombatantState CreateCombatant(string id, CombatSide side, int rank, int speed)
        {
            return new CombatantState(
                id,
                id,
                side,
                rank,
                CreateStats(maxHp: 20, attack: 5, defense: 2, speed: speed));
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
