using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;

namespace DungeonCrawler.UI
{
    public static class CombatPrototypeTurnSelectionUtility
    {
        public static List<CombatantState> GetValidBasicAttackTargets(
            CombatantState attacker,
            IReadOnlyList<CombatantState> combatants)
        {
            var result = new List<CombatantState>();

            if (attacker == null || combatants == null || !attacker.IsAlive)
            {
                return result;
            }

            for (var index = 0; index < combatants.Count; index++)
            {
                var target = combatants[index];
                if (IsValidBasicAttackTarget(attacker, target))
                {
                    result.Add(target);
                }
            }

            return result;
        }

        public static bool IsValidBasicAttackTarget(CombatantState attacker, CombatantState target)
        {
            return attacker != null
                && target != null
                && attacker.IsAlive
                && target.IsAlive
                && attacker.Side != target.Side;
        }

        public static List<CombatantState> GetLivingTargets(
            IReadOnlyList<CombatantState> combatants,
            CombatSide side)
        {
            var result = new List<CombatantState>();

            if (combatants == null)
            {
                return result;
            }

            for (var index = 0; index < combatants.Count; index++)
            {
                var combatant = combatants[index];
                if (combatant != null && combatant.Side == side && combatant.IsAlive)
                {
                    result.Add(combatant);
                }
            }

            return result;
        }

        public static CombatantState ChooseRandomTarget(IReadOnlyList<CombatantState> targets, System.Random random)
        {
            if (targets == null || targets.Count == 0)
            {
                return null;
            }

            var rng = random ?? throw new ArgumentNullException(nameof(random));
            var index = rng.Next(0, targets.Count);
            return targets[index];
        }
    }
}
