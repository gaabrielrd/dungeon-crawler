using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "DungeonThemeDefinition",
        menuName = "DungeonCrawler/Data/Dungeon/Dungeon Theme")]
    public sealed class DungeonThemeDefinition : GameDefinition
    {
        [SerializeField] private int firstFloor;
        [SerializeField] private int lastFloor;
        [SerializeField] private string visualPalette;
        [SerializeField] private Sprite background;
        [SerializeField] private AudioClip music;
        [SerializeField] private EncounterTableDefinition commonEncounters;
        [SerializeField] private EncounterTableDefinition eliteEncounters;
        [SerializeField] private EncounterTableDefinition bossEncounters;
        [SerializeField] private ShopTableDefinition shopTable;

        public int FirstFloor => firstFloor;

        public int LastFloor => lastFloor;

        public string VisualPalette => visualPalette;

        public Sprite Background => background;

        public AudioClip Music => music;

        public EncounterTableDefinition CommonEncounters => commonEncounters;

        public EncounterTableDefinition EliteEncounters => eliteEncounters;

        public EncounterTableDefinition BossEncounters => bossEncounters;

        public ShopTableDefinition ShopTable => shopTable;
    }
}
