using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    [Serializable]
    public sealed class CombatFormationState
    {
        [SerializeField] private List<CombatantState> combatants = new List<CombatantState>();

        public IReadOnlyList<CombatantState> Combatants => combatants;

        public void AddCombatant(CombatantState combatant)
        {
            if (combatant == null)
            {
                throw new ArgumentNullException(nameof(combatant));
            }

            CombatRank.Validate(combatant.Rank);

            if (CountSide(combatant.Side) >= CombatRank.MaxCombatantsPerSide)
            {
                throw new InvalidOperationException(
                    $"Combat side '{combatant.Side}' cannot contain more than {CombatRank.MaxCombatantsPerSide} combatants.");
            }

            if (ContainsRank(combatant.Side, combatant.Rank))
            {
                throw new InvalidOperationException(
                    $"Combat side '{combatant.Side}' already has a combatant at rank {combatant.Rank}.");
            }

            combatants.Add(combatant);
        }

        public int CountSide(CombatSide side)
        {
            var count = 0;

            for (var index = 0; index < combatants.Count; index++)
            {
                if (combatants[index].Side == side)
                {
                    count++;
                }
            }

            return count;
        }

        public bool ContainsRank(CombatSide side, int rank)
        {
            CombatRank.Validate(rank);

            for (var index = 0; index < combatants.Count; index++)
            {
                var combatant = combatants[index];

                if (combatant.Side == side && combatant.Rank == rank)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
