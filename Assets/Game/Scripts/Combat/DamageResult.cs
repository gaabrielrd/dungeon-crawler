namespace DungeonCrawler.Combat
{
    public readonly struct DamageResult
    {
        public DamageResult(
            CombatantState attacker,
            CombatantState target,
            int damage,
            int targetHpBefore,
            int targetHpAfter)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            TargetHpBefore = targetHpBefore;
            TargetHpAfter = targetHpAfter;
        }

        public CombatantState Attacker { get; }

        public CombatantState Target { get; }

        public int Damage { get; }

        public int TargetHpBefore { get; }

        public int TargetHpAfter { get; }
    }
}
