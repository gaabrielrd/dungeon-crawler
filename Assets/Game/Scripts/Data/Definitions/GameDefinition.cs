using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    public abstract class GameDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea]
        [SerializeField] private string description;

        // Runtime/save state must persist stable IDs, not ScriptableObject references.
        public string Id => id;

        public string DisplayName => displayName;

        public string Description => description;
    }
}
