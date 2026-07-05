using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(menuName = "DungeonCrawler/Data/Rest Site", fileName = "RestSiteDefinition")]
    public sealed class RestSiteDefinition : GameDefinition
    {
        [SerializeField][Range(0f, 1f)] private float healPercent = 1f;

        public float HealPercent => healPercent;
    }
}
