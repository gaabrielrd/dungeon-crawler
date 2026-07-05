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
    public sealed class TargetingRulesServiceTests
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
        public void SkillValidatesUserRanks()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1, 2 }, targetRanks: new[] { 1 });
            var validUser = CreateCombatant("valid_user", CombatSide.Player, rank: 2);
            var invalidUser = CreateCombatant("invalid_user", CombatSide.Player, rank: 3);

            Assert.That(service.CanUseSkill(skill, validUser).IsValid, Is.True);

            var invalidResult = service.CanUseSkill(skill, invalidUser);
            Assert.That(invalidResult.IsValid, Is.False);
            Assert.That(invalidResult.ErrorCode, Is.EqualTo(TargetingRulesService.InvalidUserRank));
        }

        [Test]
        public void ShieldBashTargetsOnlyLivingEnemiesInRanksOneAndTwo()
        {
            var service = new TargetingRulesService();
            var shieldBash = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1, 2 }, targetRanks: new[] { 1, 2 });
            var user = CreateCombatant("guardian", CombatSide.Player, rank: 1);
            var enemyRankOne = CreateCombatant("enemy_1", CombatSide.Enemy, rank: 1);
            var enemyRankTwo = CreateCombatant("enemy_2", CombatSide.Enemy, rank: 2);
            var enemyRankThree = CreateCombatant("enemy_3", CombatSide.Enemy, rank: 3);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2);
            var formation = CreateFormation(user, ally, enemyRankOne, enemyRankTwo, enemyRankThree);

            var targets = service.GetValidTargets(shieldBash, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { enemyRankOne, enemyRankTwo }));
        }

        [Test]
        public void DefenderTargetsOnlyAlliesInRankTwo()
        {
            var service = new TargetingRulesService();
            var defender = CreateSkill(SkillTargetType.Ally, userRanks: new[] { 1 }, targetRanks: new[] { 2 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var validAlly = CreateCombatant("valid_ally", CombatSide.Player, rank: 2);
            var invalidAllyRank = CreateCombatant("invalid_ally", CombatSide.Player, rank: 3);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 2);
            var formation = CreateFormation(user, validAlly, invalidAllyRank, enemy);

            var targets = service.GetValidTargets(defender, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { validAlly }));
        }

        [Test]
        public void FireballTargetsOnlyEnemiesInRankThreeOrHigher()
        {
            var service = new TargetingRulesService();
            var fireball = CreateSkill(SkillTargetType.AllEnemies, userRanks: new[] { 1 }, targetRanks: new[] { 3, 4 });
            var user = CreateCombatant("wizard", CombatSide.Player, rank: 1);
            var enemyRankOne = CreateCombatant("enemy_1", CombatSide.Enemy, rank: 1);
            var enemyRankThree = CreateCombatant("enemy_3", CombatSide.Enemy, rank: 3);
            var enemyRankFour = CreateCombatant("enemy_4", CombatSide.Enemy, rank: 4);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2);
            var formation = CreateFormation(user, enemyRankOne, enemyRankThree, enemyRankFour, ally);

            var targets = service.GetValidTargets(fireball, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { enemyRankThree, enemyRankFour }));
        }

        [Test]
        public void HealTargetsOnlyAlliesIncludingSelf()
        {
            var service = new TargetingRulesService();
            var heal = CreateSkill(SkillTargetType.AllAllies, userRanks: new[] { 2 }, targetRanks: new[] { 1, 2, 3 });
            var user = CreateCombatant("cleric", CombatSide.Player, rank: 2);
            var allyRankOne = CreateCombatant("ally_1", CombatSide.Player, rank: 1);
            var allyRankThree = CreateCombatant("ally_3", CombatSide.Player, rank: 3);
            var allyRankFour = CreateCombatant("ally_4", CombatSide.Player, rank: 4);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, allyRankOne, allyRankThree, allyRankFour, enemy);

            var targets = service.GetValidTargets(heal, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { user, allyRankOne, allyRankThree }));
        }

        [Test]
        public void BerserkTargetsAnyLivingCombatant()
        {
            var service = new TargetingRulesService();
            var berserk = CreateSkill(SkillTargetType.Any, userRanks: new[] { 1 }, targetRanks: new[] { 1, 2, 3, 4 });
            var user = CreateCombatant("berserker", CombatSide.Player, rank: 1);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 3);
            var formation = CreateFormation(user, ally, enemy);

            var targets = service.GetValidTargets(berserk, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { user, ally, enemy }));
        }

        [Test]
        public void InvalidUserRankReturnsNoTargets()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1, 2 }, targetRanks: new[] { 1, 2 });
            var user = CreateCombatant("backline_user", CombatSide.Player, rank: 4);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.Empty);
        }

        [Test]
        public void DeadEnemiesAreExcludedFromValidTargets()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1 }, targetRanks: new[] { 1, 2 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var livingEnemy = CreateCombatant("living_enemy", CombatSide.Enemy, rank: 1);
            var deadEnemy = CreateCombatant("dead_enemy", CombatSide.Enemy, rank: 2);
            deadEnemy.CurrentHp = 0;
            var formation = CreateFormation(user, livingEnemy, deadEnemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { livingEnemy }));
        }

        [Test]
        public void EnemyTargetTypeDoesNotReturnAllies()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1 }, targetRanks: new[] { 1, 2 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, ally, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { enemy }));
        }

        [Test]
        public void AllyTargetTypeReturnsLivingAlliesAndExcludesEnemies()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Ally, userRanks: new[] { 1 }, targetRanks: new[] { 2, 3 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var validAlly = CreateCombatant("valid_ally", CombatSide.Player, rank: 2);
            var invalidRankAlly = CreateCombatant("invalid_rank_ally", CombatSide.Player, rank: 4);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 2);
            var formation = CreateFormation(user, validAlly, invalidRankAlly, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { validAlly }));
        }

        [Test]
        public void SelfTargetTypeReturnsOnlyUser()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Self, userRanks: new[] { 1 }, targetRanks: Array.Empty<int>());
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 2);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, ally, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { user }));
        }

        [Test]
        public void AllEnemiesReturnsAllLivingEnemiesInValidRanks()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.AllEnemies, userRanks: new[] { 3 }, targetRanks: new[] { 1, 2, 3 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 3);
            var enemyRankOne = CreateCombatant("enemy_1", CombatSide.Enemy, rank: 1);
            var enemyRankThree = CreateCombatant("enemy_3", CombatSide.Enemy, rank: 3);
            var enemyRankFour = CreateCombatant("enemy_4", CombatSide.Enemy, rank: 4);
            var formation = CreateFormation(user, enemyRankOne, enemyRankThree, enemyRankFour);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { enemyRankOne, enemyRankThree }));
        }

        [Test]
        public void AllAlliesReturnsLivingAlliesInValidRanks()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.AllAllies, userRanks: new[] { 2 }, targetRanks: new[] { 1, 2, 3 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 2);
            var allyRankOne = CreateCombatant("ally_1", CombatSide.Player, rank: 1);
            var allyRankFour = CreateCombatant("ally_4", CombatSide.Player, rank: 4);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, allyRankOne, allyRankFour, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { user, allyRankOne }));
        }

        [Test]
        public void UserCannotUseSkillOutsideAllowedRank()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1, 2 }, targetRanks: new[] { 1 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 3);
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.Empty);
        }

        [Test]
        public void UserCannotUseSkillWhenDead()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1 }, targetRanks: new[] { 1 });
            var user = CreateCombatant("dead_user", CombatSide.Player, rank: 1);
            user.CurrentHp = 0;
            var enemy = CreateCombatant("enemy", CombatSide.Enemy, rank: 1);
            var formation = CreateFormation(user, enemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.Empty);
        }

        [Test]
        public void DeadTargetsCannotBeTargeted()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1 }, targetRanks: new[] { 1 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var livingEnemy = CreateCombatant("living_enemy", CombatSide.Enemy, rank: 1);
            var deadEnemy = CreateCombatant("dead_enemy", CombatSide.Enemy, rank: 2);
            deadEnemy.CurrentHp = 0;
            var formation = CreateFormation(user, livingEnemy, deadEnemy);

            var targets = service.GetValidTargets(skill, user, formation);

            Assert.That(targets, Is.EquivalentTo(new[] { livingEnemy }));
        }

        [Test]
        public void InvalidTargetReturnsStableErrorCode()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1 }, targetRanks: new[] { 1 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);
            var ally = CreateCombatant("ally", CombatSide.Player, rank: 1);

            var result = service.ValidateTarget(skill, user, ally);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(TargetingRulesService.InvalidTargetSide));
        }

        [Test]
        public void EmptyRankArraysAreInvalid()
        {
            var service = new TargetingRulesService();
            var skill = CreateSkill(SkillTargetType.Enemy, userRanks: Array.Empty<int>(), targetRanks: new[] { 1 });
            var user = CreateCombatant("user", CombatSide.Player, rank: 1);

            var result = service.CanUseSkill(skill, user);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(TargetingRulesService.InvalidUserRank));
        }

        [Test]
        public void ControllerUsesCurrentCombatantForTargetingQueries()
        {
            var shieldBash = CreateSkill(SkillTargetType.Enemy, userRanks: new[] { 1 }, targetRanks: new[] { 1 });
            var hero = CreateCombatant("hero", CombatSide.Player, rank: 1, speed: 10);
            var enemyRankOne = CreateCombatant("enemy_1", CombatSide.Enemy, rank: 1, speed: 1);
            var enemyRankTwo = CreateCombatant("enemy_2", CombatSide.Enemy, rank: 2, speed: 2);
            var controller = new CombatController(CreateFormation(hero, enemyRankOne, enemyRankTwo), new EventBus());

            controller.StartCombat();
            var targets = controller.GetValidTargetsForCurrentCombatant(shieldBash);
            var invalidTargetResult = controller.ValidateSkillTargetForCurrentCombatant(shieldBash, enemyRankTwo);

            Assert.That(controller.CurrentCombatant, Is.SameAs(hero));
            Assert.That(targets, Is.EquivalentTo(new[] { enemyRankOne }));
            Assert.That(invalidTargetResult.IsValid, Is.False);
            Assert.That(invalidTargetResult.ErrorCode, Is.EqualTo(TargetingRulesService.InvalidTargetRank));
        }

        private SkillDefinition CreateSkill(SkillTargetType targetType, int[] userRanks, int[] targetRanks)
        {
            var skill = ScriptableObject.CreateInstance<SkillDefinition>();
            definitions.Add(skill);

            SetPrivateField(typeof(SkillDefinition), skill, "targetType", targetType);
            SetPrivateField(typeof(SkillDefinition), skill, "validUserRanks", userRanks);
            SetPrivateField(typeof(SkillDefinition), skill, "validTargetRanks", targetRanks);

            return skill;
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

        private static CombatantState CreateCombatant(string id, CombatSide side, int rank, int speed = 1)
        {
            return new CombatantState(id, id, side, rank, CreateStats(maxHp: 20, attack: 5, defense: 2, speed: speed));
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
