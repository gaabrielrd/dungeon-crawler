using System.Reflection;
using System.Collections.Generic;
using System;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.UI;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class TestCombatDataProviderTests
    {
        private const string TestDataPath = "Assets/Game/Data/Test/test_combat_data.asset";

        [Test]
        public void TestCombatDataProvider_LoadsFromDisk()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider, Is.Not.Null, "TestCombatDataProvider asset should exist at " + TestDataPath);
        }

        [Test]
        public void TestCombatDataProvider_HasFourHeroDefinitions()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.HeroDefinitions, Has.Length.EqualTo(4));
        }

        [Test]
        public void TestCombatDataProvider_HasAtLeastOneEnemyDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.EnemyDefinitions, Has.Length.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void TestCombatDataProvider_HasAtLeastOneBossDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.BossDefinitions, Has.Length.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void TestCombatDataProvider_HasPrototypeSkillDefinitions()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.SkillDefinitions, Has.Length.EqualTo(9));
        }

        [Test]
        public void HeroDefinitions_HaveValidIds()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var expected = new[] { "guardian", "rogue", "acolyte", "arcanist" };

            for (var i = 0; i < provider.HeroDefinitions.Length; i++)
            {
                Assert.That(provider.HeroDefinitions[i].Id, Is.EqualTo(expected[i]),
                    $"Hero {i} should have id '{expected[i]}' but got '{provider.HeroDefinitions[i].Id}'");
            }
        }

        [Test]
        public void HeroDefinitions_HaveBaseStats()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);

            foreach (var hero in provider.HeroDefinitions)
            {
                Assert.That(hero.BaseStats.MaxHp, Is.GreaterThan(0), $"{hero.Id} should have MaxHp > 0");
                Assert.That(hero.BaseStats.Attack, Is.GreaterThan(0), $"{hero.Id} should have Attack > 0");
                Assert.That(hero.BaseStats.Speed, Is.GreaterThan(0), $"{hero.Id} should have Speed > 0");
            }
        }

        [Test]
        public void HeroDefinitions_HaveStartingSkills()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);

            foreach (var hero in provider.HeroDefinitions)
            {
                Assert.That(hero.StartingSkills, Has.Length.EqualTo(3), $"{hero.Id} should have basic strike plus two class skills");
                Assert.That(hero.StartingSkills[0].Id, Is.EqualTo("basic_strike"), $"{hero.Id} should start with basic strike");
            }
        }

        [Test]
        public void HeroDefinitions_HaveDocumentedStartingSkills()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);

            AssertStartingSkills(provider, "guardian",
                "basic_strike",
                "skill.guardian.guard_strike",
                "skill.guardian.iron_stance");
            AssertStartingSkills(provider, "rogue",
                "basic_strike",
                "skill.rogue.quick_cut",
                "skill.rogue.shadow_step");
            AssertStartingSkills(provider, "acolyte",
                "basic_strike",
                "skill.acolyte.minor_prayer",
                "skill.acolyte.candlelight");
            AssertStartingSkills(provider, "arcanist",
                "basic_strike",
                "skill.arcanist.profane_spark",
                "skill.arcanist.slowing_seal");
        }

        [Test]
        public void EnemyDefinition_HasValidId()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.EnemyDefinitions[0].Id, Is.EqualTo("skeleton_grunt"));
        }

        [Test]
        public void EnemyDefinition_HasBaseStats()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var enemy = provider.EnemyDefinitions[0];

            Assert.That(enemy.BaseStats.MaxHp, Is.GreaterThan(0));
            Assert.That(enemy.BaseStats.Attack, Is.GreaterThan(0));
            Assert.That(enemy.BaseStats.Speed, Is.GreaterThan(0));
        }

        [Test]
        public void EnemyDefinition_HasTheme()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.EnemyDefinitions[0].Theme, Is.Not.Null, "Enemy should have a theme assigned");
        }

        [Test]
        public void EnemyDefinition_HasSkill()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.EnemyDefinitions[0].Skills, Is.Not.Empty, "Enemy should have at least one skill");
        }

        [Test]
        public void BossDefinition_HasValidId()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            Assert.That(provider.BossDefinitions[0].Id, Is.EqualTo("boss.crypt.crypt_lord"));
        }

        [Test]
        public void BossDefinition_HasBaseStats()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var boss = provider.BossDefinitions[0];

            Assert.That(boss.BaseStats.MaxHp, Is.GreaterThan(0));
            Assert.That(boss.BaseStats.Attack, Is.GreaterThan(0));
            Assert.That(boss.BaseStats.Speed, Is.GreaterThan(0));
        }

        [Test]
        public void SkillDefinitions_HaveValidIds()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var expected = new[]
            {
                "basic_strike",
                "skill.guardian.guard_strike",
                "skill.guardian.iron_stance",
                "skill.rogue.quick_cut",
                "skill.rogue.shadow_step",
                "skill.acolyte.minor_prayer",
                "skill.acolyte.candlelight",
                "skill.arcanist.profane_spark",
                "skill.arcanist.slowing_seal",
            };

            for (var i = 0; i < provider.SkillDefinitions.Length; i++)
            {
                Assert.That(provider.SkillDefinitions[i].Id, Is.EqualTo(expected[i]),
                    $"Skill {i} should have id '{expected[i]}' but got '{provider.SkillDefinitions[i].Id}'");
            }
        }

        [Test]
        public void SkillDefinitions_HaveTargetType()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);

            foreach (var skill in provider.SkillDefinitions)
            {
                Assert.That(Enum.IsDefined(typeof(SkillTargetType), skill.TargetType), Is.True);
            }
        }

        [Test]
        public void SkillDefinitions_HaveDamageMultiplier()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);

            foreach (var skill in provider.SkillDefinitions)
            {
                Assert.That(skill.DamageMultiplier, Is.GreaterThanOrEqualTo(0f));
            }
        }

        [Test]
        public void GetHero_ReturnsCorrectDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var guardian = provider.GetHero("guardian");
            Assert.That(guardian, Is.Not.Null);
            Assert.That(guardian.Id, Is.EqualTo("guardian"));
        }

        [Test]
        public void GetEnemy_ReturnsCorrectDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var skeleton = provider.GetEnemy("skeleton_grunt");
            Assert.That(skeleton, Is.Not.Null);
            Assert.That(skeleton.Id, Is.EqualTo("skeleton_grunt"));
        }

        [Test]
        public void GetBoss_ReturnsCorrectDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var boss = provider.GetBoss("boss.crypt.crypt_lord");
            Assert.That(boss, Is.Not.Null);
            Assert.That(boss.Id, Is.EqualTo("boss.crypt.crypt_lord"));
        }

        [Test]
        public void GetSkill_ReturnsCorrectDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var basicStrike = provider.GetSkill("basic_strike");
            Assert.That(basicStrike, Is.Not.Null);
            Assert.That(basicStrike.Id, Is.EqualTo("basic_strike"));
        }

        [Test]
        public void IronStance_IsSelfTargetingUtilitySkill()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var ironStance = provider.GetSkill("skill.guardian.iron_stance");
            Assert.That(ironStance.TargetType, Is.EqualTo(SkillTargetType.Self));
            Assert.That(ironStance.DamageMultiplier, Is.Zero);
        }

        [Test]
        public void GuardStrike_RestrictedToFrontlineRanks()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var guardStrike = provider.GetSkill("skill.guardian.guard_strike");
            Assert.That(guardStrike.ValidUserRanks, Is.EquivalentTo(new[] { 1, 2 }));
            Assert.That(guardStrike.ValidTargetRanks, Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public void CombatantStateFactory_CreatesHeroFromDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var guardianDef = provider.GetHero("guardian");
            var hero = CombatantStateFactory.CreateHero(guardianDef, 1);

            Assert.That(hero, Is.Not.Null);
            Assert.That(hero.DefinitionId, Is.EqualTo("guardian"));
            Assert.That(hero.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(hero.Rank, Is.EqualTo(1));
            Assert.That(hero.MaxHp, Is.EqualTo(guardianDef.BaseStats.MaxHp));
            Assert.That(hero.CurrentHp, Is.EqualTo(guardianDef.BaseStats.MaxHp));
            Assert.That(hero.Attack, Is.EqualTo(guardianDef.BaseStats.Attack));
        }

        [Test]
        public void CombatantStateFactory_CreatesEnemyFromDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var enemyDef = provider.GetEnemy("skeleton_grunt");
            var enemy = CombatantStateFactory.CreateEnemy(enemyDef, 1);

            Assert.That(enemy, Is.Not.Null);
            Assert.That(enemy.DefinitionId, Is.EqualTo("skeleton_grunt"));
            Assert.That(enemy.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(enemy.Rank, Is.EqualTo(1));
            Assert.That(enemy.IsAlive, Is.True);
        }

        [Test]
        public void CombatantStateFactory_CreatesBossFromDefinition()
        {
            var provider = UnityEditor.AssetDatabase.LoadAssetAtPath<TestCombatDataProvider>(TestDataPath);
            var bossDef = provider.GetBoss("boss.crypt.crypt_lord");
            var boss = CombatantStateFactory.CreateBoss(bossDef, 1);

            Assert.That(boss, Is.Not.Null);
            Assert.That(boss.DefinitionId, Is.EqualTo("boss.crypt.crypt_lord"));
            Assert.That(boss.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(boss.Rank, Is.EqualTo(1));
            Assert.That(boss.MaxHp, Is.EqualTo(bossDef.BaseStats.MaxHp));
            Assert.That(boss.CurrentHp, Is.EqualTo(bossDef.BaseStats.MaxHp));
            Assert.That(boss.Attack, Is.EqualTo(bossDef.BaseStats.Attack));
        }

        private static void AssertStartingSkills(
            TestCombatDataProvider provider,
            string heroId,
            params string[] expectedSkillIds)
        {
            var hero = provider.GetHero(heroId);
            Assert.That(hero, Is.Not.Null);
            Assert.That(hero.StartingSkills, Has.Length.EqualTo(expectedSkillIds.Length));

            for (var i = 0; i < expectedSkillIds.Length; i++)
            {
                Assert.That(hero.StartingSkills[i], Is.Not.Null, $"{heroId} skill {i} should be assigned");
                Assert.That(hero.StartingSkills[i].Id, Is.EqualTo(expectedSkillIds[i]));
            }
        }
    }
}
