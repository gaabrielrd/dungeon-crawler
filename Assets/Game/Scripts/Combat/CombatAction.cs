using System;

namespace DungeonCrawler.Combat
{
    public readonly struct CombatAction
    {
        public CombatAction(
            CombatActionType type,
            CombatantState actor,
            CombatantState target,
            float damageMultiplier = 1f)
        {
            Type = type;
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            DamageMultiplier = damageMultiplier;
        }

        public CombatActionType Type { get; }

        public CombatantState Actor { get; }

        public CombatantState Target { get; }

        public float DamageMultiplier { get; }
    }
}
