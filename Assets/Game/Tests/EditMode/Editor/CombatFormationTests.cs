using System;
using System.Reflection;
using DungeonCrawler.Combat;
using DungeonCrawler.Data.Definitions;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class CombatFormationTests
    {
        [Test]
        public void FormationRejectsMoreThanFourCombatantsOnSameSide()
        {
            var formation = new CombatFormationState();

            for (var rank = CombatRank.Front; rank <= CombatRank.Back; rank++)
            {
                formation.AddCombatant(
                    new CombatantState(
                        "hero",
                        "Hero",
                        CombatSide.Player,
                        rank,
                        CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3)));
            }

            var extraCombatant = new CombatantState(
                "extra_hero",
                "Extra Hero",
                CombatSide.Player,
                CombatRank.Front,
                CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3));

            Assert.Throws<InvalidOperationException>(() => formation.AddCombatant(extraCombatant));
        }

        [Test]
        public void FormationRejectsDuplicateRankOnSameSide()
        {
            var formation = new CombatFormationState();

            formation.AddCombatant(
                new CombatantState(
                    "hero1",
                    "Hero 1",
                    CombatSide.Player,
                    CombatRank.Front,
                    CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3)));

            var duplicateRank = new CombatantState(
                "hero2",
                "Hero 2",
                CombatSide.Player,
                CombatRank.Front,
                CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3));

            Assert.Throws<InvalidOperationException>(() => formation.AddCombatant(duplicateRank));
        }

        [Test]
        public void FormationCanAddCombatantsAcrossRanks()
        {
            var formation = new CombatFormationState();

            for (var rank = CombatRank.Front; rank <= CombatRank.Back; rank++)
            {
                formation.AddCombatant(
                    new CombatantState(
                        $"hero_{rank}",
                        $"Hero {rank}",
                        CombatSide.Player,
                        rank,
                        CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3)));
            }

            Assert.That(formation.Combatants.Count, Is.EqualTo(4));
        }

        [Test]
        public void FormationCanHaveHeroesAndEnemiesSeparateSides()
        {
            var formation = new CombatFormationState();

            formation.AddCombatant(
                new CombatantState(
                    "hero",
                    "Hero",
                    CombatSide.Player,
                    CombatRank.Front,
                    CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3)));

            formation.AddCombatant(
                new CombatantState(
                    "enemy",
                    "Enemy",
                    CombatSide.Enemy,
                    CombatRank.Front,
                    CreateStats(maxHp: 20, attack: 5, defense: 2, speed: 3)));

            Assert.That(formation.CountSide(CombatSide.Player), Is.EqualTo(1));
            Assert.That(formation.CountSide(CombatSide.Enemy), Is.EqualTo(1));
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
