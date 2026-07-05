using System;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Dungeon
{
    public sealed class DungeonThemeResolver
    {
        private readonly DungeonThemeDefinition[] _themes;
        private readonly bool _sorted;

        public DungeonThemeResolver(DungeonThemeDefinition[] themes)
        {
            if (themes == null) throw new ArgumentNullException(nameof(themes));
            _themes = themes;
            _sorted = SortThemes();
        }

        public int ThemeCount => _themes.Length;

        public DungeonThemeDefinition GetTheme(int floorNumber)
        {
            for (var i = 0; i < _themes.Length; i++)
            {
                if (floorNumber >= _themes[i].FirstFloor && floorNumber <= _themes[i].LastFloor)
                {
                    return _themes[i];
                }
            }

            if (_themes.Length == 0) return null;

            // If beyond all defined ranges, return the last theme
            if (floorNumber > _themes[_themes.Length - 1].LastFloor)
            {
                return _themes[_themes.Length - 1];
            }

            return _themes[0];
        }

        public int GetThemeIndex(int floorNumber)
        {
            for (var i = 0; i < _themes.Length; i++)
            {
                if (floorNumber >= _themes[i].FirstFloor && floorNumber <= _themes[i].LastFloor)
                {
                    return i;
                }
            }

            if (_themes.Length == 0) return 0;

            if (floorNumber > _themes[_themes.Length - 1].LastFloor)
            {
                return _themes.Length - 1;
            }

            return 0;
        }

        public DungeonThemeDefinition GetNextTheme(int floorNumber)
        {
            var currentIndex = GetThemeIndex(floorNumber);
            var nextIndex = currentIndex + 1;
            return nextIndex < _themes.Length ? _themes[nextIndex] : _themes[currentIndex];
        }

        public bool IsThemeTransitionFloor(int floorNumber)
        {
            if (floorNumber < 1) return false;
            var current = GetTheme(floorNumber);
            var next = GetTheme(floorNumber + 1);
            return current != next;
        }

        public string GetThemeId(int floorNumber)
        {
            var theme = GetTheme(floorNumber);
            return theme?.Id;
        }

        public string GetThemeDisplayName(int floorNumber)
        {
            var theme = GetTheme(floorNumber);
            return theme?.DisplayName;
        }

        public string GetNextThemeId(int floorNumber)
        {
            if (!IsThemeTransitionFloor(floorNumber)) return null;
            var nextTheme = GetNextTheme(floorNumber);
            return nextTheme?.Id;
        }

        private bool SortThemes()
        {
            if (_themes.Length <= 1) return true;
            Array.Sort(_themes, (a, b) => a.FirstFloor.CompareTo(b.FirstFloor));
            return true;
        }
    }
}
