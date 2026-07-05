using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Data.State;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class HeroProgressionServiceTests
    {
        private readonly List<ScriptableObject> _definitions = new();

        [TearDown]
        public void TearDown()
        {
            for (var i = 0; i < _definitions.Count; i++)
                UnityEngine.Object.DestroyImmediate(_definitions[i]);
            _definitions.Clear();
        }

        [Test]
        public void GetAverageDamage_Level1_Returns3()
        {
            Assert.That(HeroProgressionService.GetAverageDamage(1), Is.EqualTo(3));
        }

        [Test]
        public void GetAverageDamage_Level2_Returns5()
        {
            Assert.That(HeroProgressionService.GetAverageDamage(2), Is.EqualTo(5));
        }

        [Test]
        public void GetAverageDamage_Level3_Returns8()
        {
            Assert.That(HeroProgressionService.GetAverageDamage(3), Is.EqualTo(8));
        }

        [Test]
        public void GetAverageDamage_Level5_Returns21()
        {
            Assert.That(HeroProgressionService.GetAverageDamage(5), Is.EqualTo(21));
        }

        [Test]
        public void GetAverageDamage_Level8_Returns89()
        {
            Assert.That(HeroProgressionService.GetAverageDamage(8), Is.EqualTo(89));
        }

        [Test]
        public void GetAverageDamage_Level10_Returns233()
        {
            Assert.That(HeroProgressionService.GetAverageDamage(10), Is.EqualTo(233));
        }

        [Test]
        public void GetXpToNextLevel_Level1_Returns5()
        {
            Assert.That(HeroProgressionService.GetXpToNextLevel(1), Is.EqualTo(5));
        }

        [Test]
        public void GetXpToNextLevel_Level2_Returns8()
        {
            Assert.That(HeroProgressionService.GetXpToNextLevel(2), Is.EqualTo(8));
        }

        [Test]
        public void GetXpToNextLevel_Level3_Returns13()
        {
            Assert.That(HeroProgressionService.GetXpToNextLevel(3), Is.EqualTo(13));
        }

        [Test]
        public void GetXpToNextLevel_Level5_Returns34()
        {
            Assert.That(HeroProgressionService.GetXpToNextLevel(5), Is.EqualTo(34));
        }

        [Test]
        public void GetXpToNextLevel_Level8_Returns144()
        {
            Assert.That(HeroProgressionService.GetXpToNextLevel(8), Is.EqualTo(144));
        }

        [Test]
        public void GetXpToNextLevel_Level10_Returns377()
        {
            Assert.That(HeroProgressionService.GetXpToNextLevel(10), Is.EqualTo(377));
        }

        [Test]
        public void GetTotalXpForLevel_Level1_Returns0()
        {
            Assert.That(HeroProgressionService.GetTotalXpForLevel(1), Is.EqualTo(0));
        }

        [Test]
        public void GetTotalXpForLevel_Level10_Returns602()
        {
            Assert.That(HeroProgressionService.GetTotalXpForLevel(10), Is.EqualTo(602));
        }

        [Test]
        public void TryLevelUp_SufficientXp_LevelsUpAndReturnsTrue()
        {
            var hero = CreateTestHero();

            hero.AddXp(5);

            var result = HeroProgressionService.TryLevelUp(hero);

            Assert.That(result, Is.True);
            Assert.That(hero.Level, Is.EqualTo(2));
            Assert.That(hero.CurrentXp, Is.EqualTo(0));
        }

        [Test]
        public void TryLevelUp_InsufficientXp_ReturnsFalse()
        {
            var hero = CreateTestHero();

            hero.AddXp(3);

            var result = HeroProgressionService.TryLevelUp(hero);

            Assert.That(result, Is.False);
            Assert.That(hero.Level, Is.EqualTo(1));
        }

        [Test]
        public void TryLevelUp_WithExcessXp_PreservesRemainingXp()
        {
            var hero = CreateTestHero();

            hero.AddXp(12);

            var result = HeroProgressionService.TryLevelUp(hero);

            Assert.That(result, Is.True);
            Assert.That(hero.Level, Is.EqualTo(2));
            Assert.That(hero.CurrentXp, Is.EqualTo(7));
        }

        [Test]
        public void TryLevelUp_MaxLevel_ReturnsFalse()
        {
            var hero = CreateTestHero();

            for (var i = 1; i < 10; i++)
            {
                hero.AddXp(9999);
                while (HeroProgressionService.TryLevelUp(hero)) { }
            }

            Assert.That(hero.Level, Is.EqualTo(10));
            Assert.That(hero.IsMaxLevel, Is.True);

            var result = HeroProgressionService.TryLevelUp(hero);
            Assert.That(result, Is.False);
        }

        [Test]
        public void MultiLevelUp_WithLargeXp_GainsMultipleLevels()
        {
            var hero = CreateTestHero();

            hero.AddXp(200);
            var levelsGained = 0;
            while (HeroProgressionService.TryLevelUp(hero))
            {
                levelsGained++;
            }

            Assert.That(levelsGained, Is.GreaterThanOrEqualTo(4));
            Assert.That(hero.Level, Is.GreaterThanOrEqualTo(5));
        }

        [Test]
        public void GetAvailableSkills_Level1_ReturnsNoNewSkills()
        {
            var classDef = CreateTestClassDefinition();
            var hero = CreateTestHero();

            var available = HeroProgressionService.GetAvailableSkills(classDef, hero);

            Assert.That(available, Is.Empty);
        }

        [Test]
        public void GetAvailableSkills_Level2_ReturnsLevel2Skill()
        {
            var classDef = CreateTestClassDefinition();
            var hero = CreateTestHero();
            hero.AddXp(HeroProgressionService.GetXpToNextLevel(1));
            HeroProgressionService.TryLevelUp(hero);

            var available = HeroProgressionService.GetAvailableSkills(classDef, hero);

            Assert.That(hero.Level, Is.EqualTo(2));
            Assert.That(available.Count, Is.EqualTo(1));
            Assert.That(available[0].Id, Is.EqualTo("skill.test.shield_charge"));
        }

        [Test]
        public void GetAvailableSkills_AfterLearning_DoesNotReturnLearnedSkill()
        {
            var classDef = CreateTestClassDefinition();
            var hero = CreateTestHero();
            hero.AddXp(HeroProgressionService.GetXpToNextLevel(1));
            HeroProgressionService.TryLevelUp(hero);
            hero.LearnSkill("skill.test.shield_charge");
            var available = HeroProgressionService.GetAvailableSkills(classDef, hero);

            Assert.That(available, Is.Empty);
        }

        [Test]
        public void GetAvailableSkills_Level5_ReturnsMultipleSkills()
        {
            var classDef = CreateTestClassDefinition();
            var hero = CreateTestHero();
            for (var targetLevel = 2; targetLevel <= 5; targetLevel++)
            {
                hero.AddXp(HeroProgressionService.GetXpToNextLevel(hero.Level));
                HeroProgressionService.TryLevelUp(hero);
            }

            Assert.That(hero.Level, Is.EqualTo(5));
            var available = HeroProgressionService.GetAvailableSkills(classDef, hero);

            Assert.That(available.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void GetAvailableSkills_LearnedStartingSkill_NotInAvailable()
        {
            var classDef = CreateTestClassDefinition();
            var hero = CreateTestHero();

            var available = HeroProgressionService.GetAvailableSkills(classDef, hero);

            Assert.That(available.Exists(s => s.Id == "skill.test.guard_strike"), Is.False);
        }

        [Test]
        public void GetSkillLevelMultiplier_Level1_Returns1()
        {
            var mult = HeroProgressionService.GetSkillLevelMultiplier(1);
            Assert.That(mult, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void GetSkillLevelMultiplier_Level2_Returns1Dot67()
        {
            var mult = HeroProgressionService.GetSkillLevelMultiplier(2);
            Assert.That(mult, Is.EqualTo(1.6667f).Within(0.001f));
        }

        [Test]
        public void GetSkillLevelMultiplier_Level3_Returns2Dot67()
        {
            var mult = HeroProgressionService.GetSkillLevelMultiplier(3);
            Assert.That(mult, Is.EqualTo(2.6667f).Within(0.001f));
        }

        [Test]
        public void GetSkillLevelMultiplier_Below1_ClampsTo1()
        {
            var mult = HeroProgressionService.GetSkillLevelMultiplier(0);
            Assert.That(mult, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void HeroState_BaseAverageDamage_Level1_Returns3()
        {
            var hero = CreateTestHero();
            Assert.That(hero.BaseAverageDamage, Is.EqualTo(3));
        }

        [Test]
        public void HeroState_XpToNextLevel_Level1_Returns5()
        {
            var hero = CreateTestHero();
            Assert.That(hero.XpToNextLevel, Is.EqualTo(5));
        }

        [Test]
        public void HeroState_LevelUp_IncreasesLevel()
        {
            var hero = CreateTestHero();
            Assert.That(hero.Level, Is.EqualTo(1));

            hero.AddXp(5);
            HeroProgressionService.TryLevelUp(hero);

            Assert.That(hero.Level, Is.EqualTo(2));
            Assert.That(hero.BaseAverageDamage, Is.EqualTo(5));
            Assert.That(hero.XpToNextLevel, Is.EqualTo(8));
        }

        [Test]
        public void HeroProgressionDefinition_AverageDamage_MatchesFibonacci()
        {
            var def = CreateProgressionDefinition();

            Assert.That(def.GetAverageDamage(1), Is.EqualTo(3));
            Assert.That(def.GetAverageDamage(2), Is.EqualTo(5));
            Assert.That(def.GetAverageDamage(3), Is.EqualTo(8));
            Assert.That(def.GetAverageDamage(5), Is.EqualTo(21));
            Assert.That(def.GetAverageDamage(8), Is.EqualTo(89));
            Assert.That(def.GetAverageDamage(10), Is.EqualTo(233));
        }

        [Test]
        public void HeroProgressionDefinition_XpToNext_MatchesFibonacci()
        {
            var def = CreateProgressionDefinition();

            Assert.That(def.GetXpToNextLevel(1), Is.EqualTo(5));
            Assert.That(def.GetXpToNextLevel(2), Is.EqualTo(8));
            Assert.That(def.GetXpToNextLevel(3), Is.EqualTo(13));
            Assert.That(def.GetXpToNextLevel(5), Is.EqualTo(34));
            Assert.That(def.GetXpToNextLevel(8), Is.EqualTo(144));
            Assert.That(def.GetXpToNextLevel(10), Is.EqualTo(377));
        }

        [Test]
        public void TryLevelUp_WithDefinition_SufficientXp_LevelsUp()
        {
            var def = CreateProgressionDefinition();
            var hero = CreateTestHero();

            hero.AddXp(5);

            var result = HeroProgressionService.TryLevelUp(hero, def);

            Assert.That(result, Is.True);
            Assert.That(hero.Level, Is.EqualTo(2));
        }

        [Test]
        public void HeroState_IsMaxLevel_Level10_ReturnsTrue()
        {
            var hero = CreateTestHero();
            hero.AddXp(9999);
            while (HeroProgressionService.TryLevelUp(hero)) { }

            Assert.That(hero.Level, Is.EqualTo(10));
            Assert.That(hero.IsMaxLevel, Is.True);
        }

        [Test]
        public void GetDamageRange_Level1_ReturnsCorrectRange()
        {
            Assert.That(HeroProgressionService.GetDamageRangeMin(1), Is.EqualTo(2));
            Assert.That(HeroProgressionService.GetDamageRangeMax(1), Is.EqualTo(4));
        }

        [Test]
        public void TryLevelUp_NullHero_Throws()
        {
            Assert.That(
                () => HeroProgressionService.TryLevelUp(null),
                Throws.ArgumentNullException);
        }

        private HeroState CreateTestHero()
        {
            var def = CreateTestClassDefinition();
            var hero = new HeroState(def, "Test Hero", Rarity.Common);
            _definitions.Add(def);
            return hero;
        }

        private HeroClassDefinition CreateTestClassDefinition()
        {
            var def = ScriptableObject.CreateInstance<HeroClassDefinition>();
            _definitions.Add(def);
            SetPrivateField(typeof(GameDefinition), def, "id", "test_class");
            SetPrivateField(typeof(GameDefinition), def, "displayName", "Test Class");
            SetPrivateField(typeof(HeroClassDefinition), def, "baseStats", new CombatStats(30, 5, 5, 5));
            SetPrivateField(typeof(HeroClassDefinition), def, "baseRarity", Rarity.Common);

            var strike = CreateSkill("skill.test.guard_strike", "Guard Strike");
            var stance = CreateSkill("skill.test.iron_stance", "Iron Stance");
            var shieldCharge = CreateSkill("skill.test.shield_charge", "Shield Charge");
            var protectAlly = CreateSkill("skill.test.protect_ally", "Protect Ally");
            var boneBreaker = CreateSkill("skill.test.bone_breaker", "Bone Breaker");
            var lastWall = CreateSkill("skill.test.last_wall", "Last Wall");

            SetPrivateField(typeof(HeroClassDefinition), def, "startingSkills", new[] { strike, stance });
            SetPrivateField(typeof(HeroClassDefinition), def, "skillsByLevel", new[]
            {
                CreateSkillEntry(1, strike),
                CreateSkillEntry(1, stance),
                CreateSkillEntry(2, shieldCharge),
                CreateSkillEntry(4, protectAlly),
                CreateSkillEntry(6, boneBreaker),
                CreateSkillEntry(8, lastWall),
            });

            return def;
        }

        private HeroClassDefinition.SkillByLevelEntry CreateSkillEntry(int level, SkillDefinition skill)
        {
            var entry = new HeroClassDefinition.SkillByLevelEntry();
            var levelField = typeof(HeroClassDefinition.SkillByLevelEntry)
                .GetField("level", BindingFlags.Instance | BindingFlags.NonPublic);
            var skillField = typeof(HeroClassDefinition.SkillByLevelEntry)
                .GetField("skill", BindingFlags.Instance | BindingFlags.NonPublic);
            levelField.SetValue(entry, level);
            skillField.SetValue(entry, skill);
            return entry;
        }

        private SkillDefinition CreateSkill(string id, string displayName)
        {
            var skill = ScriptableObject.CreateInstance<SkillDefinition>();
            _definitions.Add(skill);
            SetPrivateField(typeof(GameDefinition), skill, "id", id);
            SetPrivateField(typeof(GameDefinition), skill, "displayName", displayName);
            return skill;
        }

        private HeroProgressionDefinition CreateProgressionDefinition()
        {
            var def = ScriptableObject.CreateInstance<HeroProgressionDefinition>();
            _definitions.Add(def);
            SetPrivateField(typeof(GameDefinition), def, "id", "progression.test");
            SetPrivateField(typeof(GameDefinition), def, "displayName", "Test Progression");
            SetPrivateField(typeof(HeroProgressionDefinition), def, "levels", new[]
            {
                new HeroProgressionDefinition.LevelEntry(1, 3, 2, 4, 5),
                new HeroProgressionDefinition.LevelEntry(2, 5, 4, 6, 8),
                new HeroProgressionDefinition.LevelEntry(3, 8, 6, 10, 13),
                new HeroProgressionDefinition.LevelEntry(4, 13, 10, 16, 21),
                new HeroProgressionDefinition.LevelEntry(5, 21, 17, 25, 34),
                new HeroProgressionDefinition.LevelEntry(6, 34, 27, 41, 55),
                new HeroProgressionDefinition.LevelEntry(7, 55, 44, 66, 89),
                new HeroProgressionDefinition.LevelEntry(8, 89, 71, 107, 144),
                new HeroProgressionDefinition.LevelEntry(9, 144, 115, 173, 233),
                new HeroProgressionDefinition.LevelEntry(10, 233, 186, 280, 377),
            });
            return def;
        }

        private static void SetPrivateField(Type declaringType, object target, string fieldName, object value)
        {
            var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' not found on {declaringType.Name}.");
            field.SetValue(target, value);
        }
    }
}
