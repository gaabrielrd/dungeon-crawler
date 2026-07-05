using System;

namespace DungeonCrawler.Combat
{
    public sealed class DamageResolver
    {
        public DamageResult Resolve(CombatAction action)
        {
            switch (action.Type)
            {
                case CombatActionType.BasicAttack:
                    return ResolveBasicAttack(action.Actor, action.Target);
                case CombatActionType.Skill:
                    return ResolveSkillAttack(action.Actor, action.Target, action.DamageMultiplier);
                default:
                    throw new InvalidOperationException($"Unsupported combat action '{action.Type}'.");
            }
        }

        public DamageResult ResolveBasicAttack(CombatantState attacker, CombatantState target)
        {
            return ResolveAttack(attacker, target, 1f, "Basic attacks must target an opposing side.");
        }

        public DamageResult ResolveSkillAttack(
            CombatantState attacker,
            CombatantState target,
            float damageMultiplier)
        {
            return ResolveAttack(attacker, target, damageMultiplier, null);
        }

        private DamageResult ResolveAttack(
            CombatantState attacker,
            CombatantState target,
            float damageMultiplier,
            string sideGuardMessage)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException(nameof(attacker));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!attacker.IsAlive)
            {
                throw new InvalidOperationException("Dead combatants cannot attack.");
            }

            if (!target.IsAlive)
            {
                throw new InvalidOperationException("Dead combatants cannot be targeted.");
            }

            if (sideGuardMessage != null && attacker.Side == target.Side)
            {
                throw new InvalidOperationException(sideGuardMessage);
            }

            var targetHpBefore = target.CurrentHp;
            var baseDamage = Math.Max(1, attacker.Attack - target.Defense);
            var damage = damageMultiplier <= 0f ? 0 : Math.Max(1, (int)(baseDamage * damageMultiplier));
            target.CurrentHp -= damage;

            return new DamageResult(attacker, target, damage, targetHpBefore, target.CurrentHp);
        }
    }
}
