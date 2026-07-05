using System;
using System.Collections.Generic;
using System.Reflection;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;
using NUnit.Framework;
using UnityEngine;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class DungeonThemeResolverTests
    {
        private readonly List<ScriptableObject> _definitions = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var def in _definitions)
            {
                if (def != null) UnityEngine.Object.DestroyImmediate(def);
            }
            _definitions.Clear();
        }

        [Test]
        public void Floor1_UsesThemeOne()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(1);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme A"));
            Assert.That(theme.Id, Is.EqualTo("theme_a"));
        }

        [Test]
        public void Floor20_UsesThemeOne()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(20);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme A"));
        }

        [Test]
        public void Floor21_UsesThemeTwo()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(21);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme B"));
        }

        [Test]
        public void Floor40_UsesThemeTwo()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(40);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme B"));
        }

        [Test]
        public void Floor41_UsesThemeThree()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(41);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme C"));
        }

        [Test]
        public void Floor60_UsesThemeThree()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(60);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme C"));
        }

        [Test]
        public void Floor20_IsThemeTransition()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.IsThemeTransitionFloor(20), Is.True);
        }

        [Test]
        public void Floor21_IsNotThemeTransition()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.IsThemeTransitionFloor(21), Is.False);
        }

        [Test]
        public void Floor40_IsThemeTransition()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.IsThemeTransitionFloor(40), Is.True);
        }

        [Test]
        public void Floor41_IsNotThemeTransition()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.IsThemeTransitionFloor(41), Is.False);
        }

        [Test]
        public void TransitionFromFloor20_NextThemeIsThemeTwo()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var next = resolver.GetNextTheme(20);

            Assert.That(next.DisplayName, Is.EqualTo("Theme B"));
        }

        [Test]
        public void TransitionFromFloor40_NextThemeIsThemeThree()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var next = resolver.GetNextTheme(40);

            Assert.That(next.DisplayName, Is.EqualTo("Theme C"));
        }

        [Test]
        public void GetNextThemeId_ReturnsNull_WhenNoTransition()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.GetNextThemeId(1), Is.Null);
            Assert.That(resolver.GetNextThemeId(21), Is.Null);
            Assert.That(resolver.GetNextThemeId(41), Is.Null);
        }

        [Test]
        public void GetNextThemeId_ReturnsNextId_AtTransition()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.GetNextThemeId(20), Is.EqualTo("theme_b"));
            Assert.That(resolver.GetNextThemeId(40), Is.EqualTo("theme_c"));
        }

        [Test]
        public void Floor99_UsesLastTheme()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            var theme = resolver.GetTheme(99);

            Assert.That(theme.DisplayName, Is.EqualTo("Theme C"));
        }

        [Test]
        public void GetThemeIndex_ReturnsCorrectIndex()
        {
            var themes = CreateThreeThemes();
            var resolver = new DungeonThemeResolver(themes);

            Assert.That(resolver.GetThemeIndex(1), Is.EqualTo(0));
            Assert.That(resolver.GetThemeIndex(21), Is.EqualTo(1));
            Assert.That(resolver.GetThemeIndex(41), Is.EqualTo(2));
        }

        [Test]
        public void NullThemes_Throws()
        {
            Assert.That(() => new DungeonThemeResolver(null), Throws.ArgumentNullException);
        }

        [Test]
        public void EmptyThemes_ReturnsNull()
        {
            var resolver = new DungeonThemeResolver(new DungeonThemeDefinition[0]);

            Assert.That(resolver.GetTheme(1), Is.Null);
            Assert.That(resolver.ThemeCount, Is.EqualTo(0));
        }

        [Test]
        public void SingleTheme_AllFloorsUseIt()
        {
            var theme = CreateTheme("theme_only", "Only Theme", 1, 100);
            var resolver = new DungeonThemeResolver(new[] { theme });

            Assert.That(resolver.GetTheme(1).Id, Is.EqualTo("theme_only"));
            Assert.That(resolver.GetTheme(50).Id, Is.EqualTo("theme_only"));
            Assert.That(resolver.GetTheme(100).Id, Is.EqualTo("theme_only"));
            Assert.That(resolver.IsThemeTransitionFloor(1), Is.False);
            Assert.That(resolver.IsThemeTransitionFloor(100), Is.False);
        }

        [Test]
        public void Themes_AreSortedByFirstFloor()
        {
            var themeB = CreateTheme("theme_b", "Theme B", 21, 40);
            var themeA = CreateTheme("theme_a", "Theme A", 1, 20);
            var themeC = CreateTheme("theme_c", "Theme C", 41, 60);

            var resolver = new DungeonThemeResolver(new[] { themeB, themeA, themeC });

            Assert.That(resolver.GetTheme(1).Id, Is.EqualTo("theme_a"));
            Assert.That(resolver.GetTheme(21).Id, Is.EqualTo("theme_b"));
            Assert.That(resolver.GetTheme(41).Id, Is.EqualTo("theme_c"));
        }

        private DungeonThemeDefinition[] CreateThreeThemes()
        {
            var a = CreateTheme("theme_a", "Theme A", 1, 20);
            var b = CreateTheme("theme_b", "Theme B", 21, 40);
            var c = CreateTheme("theme_c", "Theme C", 41, 60);
            return new[] { a, b, c };
        }

        private DungeonThemeDefinition CreateTheme(string id, string displayName, int firstFloor, int lastFloor)
        {
            var theme = ScriptableObject.CreateInstance<DungeonThemeDefinition>();
            _definitions.Add(theme);
            SetPrivateField(typeof(GameDefinition), theme, "id", id);
            SetPrivateField(typeof(GameDefinition), theme, "displayName", displayName);
            SetPrivateField(typeof(DungeonThemeDefinition), theme, "firstFloor", firstFloor);
            SetPrivateField(typeof(DungeonThemeDefinition), theme, "lastFloor", lastFloor);
            return theme;
        }

        private static void SetPrivateField(Type declaringType, object target, string fieldName, object value)
        {
            var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"Field '{fieldName}' was not found on {declaringType.Name}.");
            field.SetValue(target, value);
        }
    }
}
