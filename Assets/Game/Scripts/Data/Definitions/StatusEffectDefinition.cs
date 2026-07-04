using UnityEngine;

namespace DungeonCrawler.Data.Definitions
{
    [CreateAssetMenu(
        fileName = "StatusEffectDefinition",
        menuName = "DungeonCrawler/Data/Combat/Status Effect")]
    public sealed class StatusEffectDefinition : GameDefinition
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private StatusEffectType effectType;
        [Min(0)]
        [SerializeField] private int defaultDurationTurns;
        [SerializeField] private StatusEffectStackPolicy stackPolicy;
        [SerializeField] private string[] tags;

        public Sprite Icon => icon;

        public StatusEffectType EffectType => effectType;

        public int DefaultDurationTurns => defaultDurationTurns;

        public StatusEffectStackPolicy StackPolicy => stackPolicy;

        public string[] Tags => tags;
    }
}
