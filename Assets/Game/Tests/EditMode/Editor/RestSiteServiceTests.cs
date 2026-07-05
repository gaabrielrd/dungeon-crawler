using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class RestSiteServiceTests
    {
        private readonly List<ScriptableObject> _definitions = new();
        private RestSiteService _defaultService;

        [SetUp]
        public void SetUp()
        {
            _defaultService = new RestSiteService();
            ServiceRegistry.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceRegistry.Clear();

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
        public void ShouldEnterRestSite_Floor10_ReturnsTrue()
        {
            var floor = CreateFloor(10, true);

            Assert.That(_defaultService.ShouldEnterRestSite(floor), Is.True);
        }

        [Test]
        public void ShouldEnterRestSite_Floor5_ReturnsFalse()
        {
            var floor = CreateFloor(5, false);

            Assert.That(_defaultService.ShouldEnterRestSite(floor), Is.False);
        }

        [Test]
        public void ShouldEnterRestSite_Floor15_ReturnsFalse()
        {
            var floor = CreateFloor(15, false);

            Assert.That(_defaultService.ShouldEnterRestSite(floor), Is.False);
        }

        [Test]
        public void Rest_HealsPartyToFull_WithDefaultConfig()
        {
            var party = CreateParty(3, 20);

            foreach (var member in party)
            {
                member.CurrentHp = 5;
            }

            _defaultService.Rest(party);

            for (var i = 0; i < party.Count; i++)
            {
                Assert.That(party[i].CurrentHp, Is.EqualTo(20));
            }
        }

        [Test]
        public void Rest_HealsPartial_With50PercentConfig()
        {
            var config = CreateRestSiteDefinition(0.5f);
            var service = new RestSiteService(config);
            var party = CreateParty(1, 20);
            party[0].CurrentHp = 10;

            service.Rest(party);

            Assert.That(party[0].CurrentHp, Is.EqualTo(20));
        }

        [Test]
        public void Rest_DoesNotHealDeadCombatants()
        {
            var party = CreateParty(1, 20);
            party[0].CurrentHp = 0;

            _defaultService.Rest(party);

            Assert.That(party[0].CurrentHp, Is.EqualTo(0));
            Assert.That(party[0].IsAlive, Is.False);
        }

        [Test]
        public void Rest_With50Percent_HealsHalfOfMaxHp()
        {
            var config = CreateRestSiteDefinition(0.5f);
            var service = new RestSiteService(config);
            var party = CreateParty(1, 100);
            party[0].CurrentHp = 40;

            service.Rest(party);

            Assert.That(party[0].CurrentHp, Is.EqualTo(90));
        }

        [Test]
        public void Rest_DoesNotOverheal()
        {
            var party = CreateParty(1, 20);
            party[0].CurrentHp = 18;

            _defaultService.Rest(party);

            Assert.That(party[0].CurrentHp, Is.EqualTo(20));
        }

        [Test]
        public void Rest_HealsMultiplePartyMembers()
        {
            var party = CreateParty(4, 30);

            party[0].CurrentHp = 10;
            party[1].CurrentHp = 20;
            party[2].CurrentHp = 15;
            party[3].CurrentHp = 25;

            _defaultService.Rest(party);

            for (var i = 0; i < party.Count; i++)
            {
                Assert.That(party[i].CurrentHp, Is.EqualTo(30));
            }
        }

        [Test]
        public void Rest_SetsHasRestTakenToTrue()
        {
            var party = CreateParty(1, 20);

            Assert.That(_defaultService.HasRestTaken, Is.False);

            _defaultService.Rest(party);

            Assert.That(_defaultService.HasRestTaken, Is.True);
        }

        [Test]
        public void ResetRestTaken_ClearsFlag()
        {
            var party = CreateParty(1, 20);
            _defaultService.Rest(party);
            Assert.That(_defaultService.HasRestTaken, Is.True);

            _defaultService.ResetRestTaken();

            Assert.That(_defaultService.HasRestTaken, Is.False);
        }

        [Test]
        public void ShouldEnterRestSite_NullFloor_Throws()
        {
            Assert.That(() => _defaultService.ShouldEnterRestSite(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Rest_NullParty_Throws()
        {
            Assert.That(() => _defaultService.Rest(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Rest_EmptyParty_DoesNotThrow()
        {
            var party = new List<CombatantState>();

            Assert.DoesNotThrow(() => _defaultService.Rest(party));
        }

        [Test]
        public void Service_Registered_CanBeResolved()
        {
            ServiceRegistry.Register<IRestSiteService>(_defaultService);

            var resolved = ServiceRegistry.Resolve<IRestSiteService>();

            Assert.That(resolved, Is.SameAs(_defaultService));
        }

        private static GeneratedFloor CreateFloor(int floorNumber, bool hasRestingSite)
        {
            return new GeneratedFloor(floorNumber, FloorType.Boss, hasRestingSite, false, "crypt", null, 0);
        }

        private static List<CombatantState> CreateParty(int count, int maxHp)
        {
            var party = new List<CombatantState>();
            for (var i = 0; i < count; i++)
            {
                party.Add(new CombatantState(
                    $"hero.{i}",
                    $"Hero {i + 1}",
                    CombatSide.Player,
                    i + 1,
                    CreateStats(maxHp, 5, 2, 5)));
            }

            return party;
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

        private RestSiteDefinition CreateRestSiteDefinition(float healPercent)
        {
            var def = ScriptableObject.CreateInstance<RestSiteDefinition>();
            _definitions.Add(def);
            SetPrivateField(typeof(GameDefinition), def, "id", "rest.site.test");
            SetPrivateField(typeof(GameDefinition), def, "displayName", "Test Rest Site");
            SetPrivateField(typeof(RestSiteDefinition), def, "healPercent", healPercent);
            return def;
        }

        private static void SetPrivateField(Type declaringType, object target, string fieldName, object value)
        {
            var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on {declaringType.Name}.");
            field.SetValue(target, value);
        }
    }
}
