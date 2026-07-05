using DungeonCrawler.Dungeon;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class FloorGeneratorTests
    {
        private const string TestSeed = "test-seed-123";
        private const string DefaultTheme = "crypt";

        private FloorGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new FloorGenerator();
        }

        [Test]
        public void Floor1_IsCombat()
        {
            var floor = _generator.GenerateFloor(TestSeed, 1, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Combat));
            Assert.That(floor.FloorNumber, Is.EqualTo(1));
            Assert.That(floor.HasRestingSite, Is.False);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor2_IsCombat()
        {
            var floor = _generator.GenerateFloor(TestSeed, 2, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Combat));
            Assert.That(floor.HasRestingSite, Is.False);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor5_IsBoss()
        {
            var floor = _generator.GenerateFloor(TestSeed, 5, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.False);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor10_IsBoss_WithRestingSite()
        {
            var floor = _generator.GenerateFloor(TestSeed, 10, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.True);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor15_IsBoss()
        {
            var floor = _generator.GenerateFloor(TestSeed, 15, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.False);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor20_IsBoss_WithRestingSite_AndThemeTransition()
        {
            var floor = _generator.GenerateFloor(TestSeed, 20, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.True);
            Assert.That(floor.IsThemeTransition, Is.True);
        }

        [Test]
        public void Floor25_IsBoss_NoResting()
        {
            var floor = _generator.GenerateFloor(TestSeed, 25, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.False);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor30_IsBoss_WithRestingSite()
        {
            var floor = _generator.GenerateFloor(TestSeed, 30, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.True);
            Assert.That(floor.IsThemeTransition, Is.False);
        }

        [Test]
        public void Floor40_IsBoss_WithRestingSite_AndThemeTransition()
        {
            var floor = _generator.GenerateFloor(TestSeed, 40, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Boss));
            Assert.That(floor.HasRestingSite, Is.True);
            Assert.That(floor.IsThemeTransition, Is.True);
        }

        [Test]
        public void SameSeed_ReturnsSameResult()
        {
            var floorA = _generator.GenerateFloor(TestSeed, 7, DefaultTheme);
            var floorB = _generator.GenerateFloor(TestSeed, 7, DefaultTheme);

            Assert.That(floorA.LocalSeed, Is.EqualTo(floorB.LocalSeed));
            Assert.That(floorA.PrimaryType, Is.EqualTo(floorB.PrimaryType));
            Assert.That(floorA.HasRestingSite, Is.EqualTo(floorB.HasRestingSite));
            Assert.That(floorA.IsThemeTransition, Is.EqualTo(floorB.IsThemeTransition));
            Assert.That(floorA.ThemeId, Is.EqualTo(floorB.ThemeId));
        }

        [Test]
        public void DifferentSeeds_MayDiffer()
        {
            var floorA = _generator.GenerateFloor("seed-alpha", 7, DefaultTheme);
            var floorB = _generator.GenerateFloor("seed-beta", 7, DefaultTheme);

            Assert.That(floorA.LocalSeed, Is.Not.EqualTo(floorB.LocalSeed));
        }

        [Test]
        public void Floor20_ThemeChanges()
        {
            var floor = _generator.GenerateFloor(TestSeed, 20, DefaultTheme);

            Assert.That(floor.IsThemeTransition, Is.True);
            Assert.That(floor.NextThemeId, Is.Not.Null);
            Assert.That(floor.NextThemeId, Is.Not.EqualTo(DefaultTheme));
        }

        [Test]
        public void Floor20_ThemeIdEqualsNextThemeId()
        {
            var floor = _generator.GenerateFloor(TestSeed, 20, DefaultTheme);

            Assert.That(floor.IsThemeTransition, Is.True);
            Assert.That(floor.ThemeId, Is.EqualTo(floor.NextThemeId));
        }

        [Test]
        public void NonMilestoneFloor_UsesCurrentTheme()
        {
            var floor = _generator.GenerateFloor(TestSeed, 7, DefaultTheme);

            Assert.That(floor.ThemeId, Is.EqualTo(DefaultTheme));
            Assert.That(floor.NextThemeId, Is.Null);
        }

        [Test]
        public void FloorNumberZero_IsCombat()
        {
            var floor = _generator.GenerateFloor(TestSeed, 0, DefaultTheme);

            Assert.That(floor.PrimaryType, Is.EqualTo(FloorType.Combat));
            Assert.That(floor.HasRestingSite, Is.False);
            Assert.That(floor.IsThemeTransition, Is.False);
        }
    }
}
