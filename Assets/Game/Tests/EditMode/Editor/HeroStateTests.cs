using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Data.State;
using DungeonCrawler.Core.Services;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class HeroStateTests
    {
        private readonly List<ScriptableObject> _definitions = new List<ScriptableObject>();

        [TearDown]
        public void TearDown()
        {
            for (var i = 0; i < _definitions.Count; i++)
                UnityEngine.Object.DestroyImmediate(_definitions[i]);
            _definitions.Clear();
        }

        [Test]
        public void CreateHeroStateFromDefinitionCopiesStats()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            Assert.That(hero.ClassId, Is.EqualTo("guardian"));
            Assert.That(hero.HeroName, Is.EqualTo("Aldren Voss"));
            Assert.That(hero.Level, Is.EqualTo(1));
            Assert.That(hero.MaxHp, Is.EqualTo(45));
            Assert.That(hero.CurrentHp, Is.EqualTo(45));
            Assert.That(hero.Attack, Is.EqualTo(7));
            Assert.That(hero.Defense, Is.EqualTo(10));
            Assert.That(hero.Speed, Is.EqualTo(4));
            Assert.That(hero.Rarity, Is.EqualTo(Rarity.Common));
        }

        [Test]
        public void MultipleHeroesOfSameClassHaveIndependentStats()
        {
            var definition = CreateRogueDefinition();

            var heroA = new HeroState(definition, "Neria Vale", Rarity.Common);
            var heroB = new HeroState(definition, "Sylas Crowe", Rarity.Uncommon);

            Assert.That(heroA.MaxHp, Is.EqualTo(heroB.MaxHp));
            Assert.That(heroA.Attack, Is.EqualTo(heroB.Attack));

            heroA.TakeDamage(10);

            Assert.That(heroA.CurrentHp, Is.LessThan(heroB.CurrentHp));
            Assert.That(heroB.CurrentHp, Is.EqualTo(heroB.MaxHp));
        }

        [Test]
        public void MutatingHeroCurrentHpDoesNotModifyDefinitionStats()
        {
            var definition = CreateGuardianDefinition();
            var originalHp = definition.BaseStats.MaxHp;

            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);
            hero.TakeDamage(20);

            Assert.That(hero.CurrentHp, Is.EqualTo(25));
            Assert.That(definition.BaseStats.MaxHp, Is.EqualTo(originalHp));
        }

        [Test]
        public void AddingXpDoesNotModifyDefinition()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            hero.AddXp(150);

            Assert.That(hero.CurrentXp, Is.EqualTo(150));
        }

        [Test]
        public void StartingSkillsAreCopiedFromDefinition()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            Assert.That(hero.LearnedSkillIds.Count, Is.EqualTo(2));
            Assert.That(hero.HasSkill("guard_strike"), Is.True);
            Assert.That(hero.HasSkill("iron_stance"), Is.True);
        }

        [Test]
        public void LearnSkillAddsToHeroOnly()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            var result = hero.LearnSkill("shield_charge");

            Assert.That(result, Is.True);
            Assert.That(hero.HasSkill("shield_charge"), Is.True);
        }

        [Test]
        public void LearnDuplicateSkillReturnsFalse()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            var result = hero.LearnSkill("guard_strike");

            Assert.That(result, Is.False);
        }

        [Test]
        public void CreateCombatantStatePreservesStats()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            var combatant = hero.CreateCombatantState(CombatRank.Front);

            Assert.That(combatant.DefinitionId, Is.EqualTo("guardian"));
            Assert.That(combatant.DisplayName, Is.EqualTo("Aldren Voss"));
            Assert.That(combatant.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(combatant.Rank, Is.EqualTo(CombatRank.Front));
            Assert.That(combatant.MaxHp, Is.EqualTo(45));
            Assert.That(combatant.CurrentHp, Is.EqualTo(45));
            Assert.That(combatant.Attack, Is.EqualTo(7));
            Assert.That(combatant.Defense, Is.EqualTo(10));
            Assert.That(combatant.Speed, Is.EqualTo(4));
        }

        [Test]
        public void SyncAfterCombatUpdatesCurrentHp()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);
            var combatant = hero.CreateCombatantState(CombatRank.Front);

            combatant.CurrentHp = 15;
            hero.SyncAfterCombat(combatant);

            Assert.That(hero.CurrentHp, Is.EqualTo(15));
        }

        [Test]
        public void PartySupportsMultipleHeroesOfSameClass()
        {
            var definition = CreateRogueDefinition();

            var heroA = new HeroState(definition, "Neria Vale", Rarity.Common) { IsInParty = true, PartyRank = 2 };
            var heroB = new HeroState(definition, "Sylas Crowe", Rarity.Uncommon) { IsInParty = true, PartyRank = 3 };

            var party = new List<HeroState> { heroA, heroB };

            Assert.That(party.Count, Is.EqualTo(2));
            Assert.That(party[0].HeroName, Is.EqualTo("Neria Vale"));
            Assert.That(party[1].HeroName, Is.EqualTo("Sylas Crowe"));

            var combatA = party[0].CreateCombatantState(party[0].PartyRank);
            var combatB = party[1].CreateCombatantState(party[1].PartyRank);

            Assert.That(combatA.Rank, Is.EqualTo(2));
            Assert.That(combatB.Rank, Is.EqualTo(3));

            combatA.CurrentHp = 5;
            combatB.CurrentHp = 20;

            Assert.That(combatA.CurrentHp, Is.LessThan(combatB.CurrentHp));
        }

        [Test]
        public void HealDoesNotExceedMaxHp()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            hero.TakeDamage(10);
            hero.Heal(20);

            Assert.That(hero.CurrentHp, Is.EqualTo(hero.MaxHp));
        }

        [Test]
        public void TakeDamageDoesNotGoBelowZero()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            hero.TakeDamage(999);

            Assert.That(hero.CurrentHp, Is.EqualTo(0));
            Assert.That(hero.IsDead, Is.True);
        }

        [Test]
        public void BaseAverageDamage_Level1_Returns3()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            Assert.That(hero.BaseAverageDamage, Is.EqualTo(3));
        }

        [Test]
        public void XpToNextLevel_Level1_Returns5()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            Assert.That(hero.XpToNextLevel, Is.EqualTo(5));
        }

        [Test]
        public void LevelUp_IncreasesLevelAndUpdatesProperties()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            hero.AddXp(5);
            var leveledUp = HeroProgressionService.TryLevelUp(hero);

            Assert.That(leveledUp, Is.True);
            Assert.That(hero.Level, Is.EqualTo(2));
            Assert.That(hero.BaseAverageDamage, Is.EqualTo(5));
            Assert.That(hero.XpToNextLevel, Is.EqualTo(8));
        }

        [Test]
        public void LevelUp_MaxLevel_DoesNotExceed10()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            hero.AddXp(9999);
            while (HeroProgressionService.TryLevelUp(hero)) { }

            Assert.That(hero.Level, Is.EqualTo(10));
            Assert.That(hero.IsMaxLevel, Is.True);
            Assert.That(hero.BaseAverageDamage, Is.EqualTo(233));
        }

        [Test]
        public void AddXp_NegativeAmount_DoesNotGoBelowZero()
        {
            var definition = CreateGuardianDefinition();
            var hero = new HeroState(definition, "Aldren Voss", Rarity.Common);

            hero.AddXp(10);
            hero.AddXp(-15);

            Assert.That(hero.CurrentXp, Is.EqualTo(0));
        }

        [Test]
        public void CreateCombatantStateFromFactoryUsesHeroStateStats()
        {
            var definition = CreateAcolyteDefinition();
            var hero = new HeroState(definition, "Mirella Thorne", Rarity.Common);
            hero.TakeDamage(8);

            var combatant = CombatantStateFactory.CreateHeroFromState(hero, CombatRank.Back);

            Assert.That(combatant.DisplayName, Is.EqualTo("Mirella Thorne"));
            Assert.That(combatant.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(combatant.MaxHp, Is.EqualTo(32));
            Assert.That(combatant.CurrentHp, Is.EqualTo(24));
        }

        [Test]
        public void RosterCanBeStoredInDungeonRunState()
        {
            var guardianDef = CreateGuardianDefinition();
            var rogueDef = CreateRogueDefinition();

            var roster = new List<HeroState>
            {
                new HeroState(guardianDef, "Aldren Voss", Rarity.Common) { IsInParty = true, PartyRank = 1 },
                new HeroState(rogueDef, "Neria Vale", Rarity.Uncommon) { IsInParty = true, PartyRank = 3 }
            };

            var partyCombatants = new List<CombatantState>();
            for (var i = 0; i < roster.Count; i++)
            {
                if (roster[i].IsInParty)
                    partyCombatants.Add(roster[i].CreateCombatantState(roster[i].PartyRank));
            }

            var runState = DungeonRunState.CreateNew("test_seed", roster, partyCombatants);

            Assert.That(runState.Roster.Count, Is.EqualTo(2));
            Assert.That(runState.Party.Count, Is.EqualTo(2));
            Assert.That(runState.Roster[0].HeroName, Is.EqualTo("Aldren Voss"));
            Assert.That(runState.Roster[1].HeroName, Is.EqualTo("Neria Vale"));
        }

        private HeroClassDefinition CreateGuardianDefinition()
        {
            var def = ScriptableObject.CreateInstance<HeroClassDefinition>();
            _definitions.Add(def);
            SetGameDefinitionFields(def, "guardian", "Guardian");
            SetPrivateField(typeof(HeroClassDefinition), def, "role", "Tank");
            SetPrivateField(typeof(HeroClassDefinition), def, "preferredRanks", new[] { 1, 2 });
            SetPrivateField(typeof(HeroClassDefinition), def, "validRanks", new[] { 1, 2 });
            SetPrivateField(typeof(HeroClassDefinition), def, "baseStats", new CombatStats(45, 7, 10, 4));
            SetPrivateField(typeof(HeroClassDefinition), def, "baseRarity", Rarity.Common);
            SetPrivateField(typeof(HeroClassDefinition), def, "namePool", new[] { "Aldren Voss", "Garrick Holt" });

            var strike = CreateSkill("guard_strike", "Guard Strike");
            var stance = CreateSkill("iron_stance", "Iron Stance");
            SetPrivateField(typeof(HeroClassDefinition), def, "startingSkills", new[] { strike, stance });

            return def;
        }

        private HeroClassDefinition CreateRogueDefinition()
        {
            var def = ScriptableObject.CreateInstance<HeroClassDefinition>();
            _definitions.Add(def);
            SetGameDefinitionFields(def, "rogue", "Rogue");
            SetPrivateField(typeof(HeroClassDefinition), def, "role", "Damage");
            SetPrivateField(typeof(HeroClassDefinition), def, "preferredRanks", new[] { 2, 3 });
            SetPrivateField(typeof(HeroClassDefinition), def, "validRanks", new[] { 1, 2, 3 });
            SetPrivateField(typeof(HeroClassDefinition), def, "baseStats", new CombatStats(28, 12, 4, 9));
            SetPrivateField(typeof(HeroClassDefinition), def, "baseRarity", Rarity.Common);
            SetPrivateField(typeof(HeroClassDefinition), def, "namePool", new[] { "Neria Vale", "Sylas Crowe" });

            var strike = CreateSkill("quick_cut", "Quick Cut");
            var step = CreateSkill("shadow_step", "Shadow Step");
            SetPrivateField(typeof(HeroClassDefinition), def, "startingSkills", new[] { strike, step });

            return def;
        }

        private HeroClassDefinition CreateAcolyteDefinition()
        {
            var def = ScriptableObject.CreateInstance<HeroClassDefinition>();
            _definitions.Add(def);
            SetGameDefinitionFields(def, "acolyte", "Acolyte");
            SetPrivateField(typeof(HeroClassDefinition), def, "role", "Support");
            SetPrivateField(typeof(HeroClassDefinition), def, "preferredRanks", new[] { 3, 4 });
            SetPrivateField(typeof(HeroClassDefinition), def, "validRanks", new[] { 1, 2, 3, 4 });
            SetPrivateField(typeof(HeroClassDefinition), def, "baseStats", new CombatStats(32, 5, 10, 6));
            SetPrivateField(typeof(HeroClassDefinition), def, "baseRarity", Rarity.Common);
            SetPrivateField(typeof(HeroClassDefinition), def, "namePool", new[] { "Mirella Thorne", "Elianora Voss" });

            var prayer = CreateSkill("lesser_prayer", "Lesser Prayer");
            var light = CreateSkill("candle_light", "Candle Light");
            SetPrivateField(typeof(HeroClassDefinition), def, "startingSkills", new[] { prayer, light });

            return def;
        }

        private SkillDefinition CreateSkill(string id, string displayName)
        {
            var skill = ScriptableObject.CreateInstance<SkillDefinition>();
            _definitions.Add(skill);
            SetGameDefinitionFields(skill, id, displayName);
            return skill;
        }

        private static void SetGameDefinitionFields(GameDefinition def, string id, string displayName)
        {
            SetPrivateField(typeof(GameDefinition), def, "id", id);
            SetPrivateField(typeof(GameDefinition), def, "displayName", displayName);
        }

        private static void SetPrivateField(Type declaringType, object target, string fieldName, object value)
        {
            var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' not found on {declaringType.Name}.");
            field.SetValue(target, value);
        }
    }
}
