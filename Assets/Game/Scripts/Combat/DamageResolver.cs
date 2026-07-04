using System;

namespace DungeonCrawler.Combat
{
    public sealed class DamageResolver
    {
        public DamageResult Resolve(CombatAction action)
        {
            if (action.Type != CombatActionType.BasicAttack)
            {
                throw new InvalidOperationException($"Unsupported combat action '{action.Type}'.");
            }

            return ResolveBasicAttack(action.Actor, action.Target);
        }

        public DamageResult ResolveBasicAttack(CombatantState attacker, CombatantState target)
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

            if (attacker.Side == target.Side)
            {
                throw new InvalidOperationException("Basic attacks must target an opposing side.");
            }

            var targetHpBefore = target.CurrentHp;
            var damage = Math.Max(1, attacker.Attack - target.Defense);
            target.CurrentHp -= damage;

            return new DamageResult(attacker, target, damage, targetHpBefore, target.CurrentHp);
        }
    }
}
