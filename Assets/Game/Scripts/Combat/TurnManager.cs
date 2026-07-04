using System;
using System.Collections.Generic;

namespace DungeonCrawler.Combat
{
    public sealed class TurnManager
    {
        private readonly IReadOnlyList<CombatantState> _combatants;
        private readonly List<CombatantState> _turnOrder = new List<CombatantState>();
        private int _nextTurnIndex;

        public TurnManager(CombatFormationState formation)
            : this(formation?.Combatants)
        {
        }

        public TurnManager(IReadOnlyList<CombatantState> combatants)
        {
            _combatants = combatants ?? throw new ArgumentNullException(nameof(combatants));
            RebuildTurnOrder();
        }

        public IReadOnlyList<CombatantState> TurnOrder => _turnOrder;

        public CombatantState GetNextCombatant()
        {
            while (true)
            {
                if (_turnOrder.Count == 0 || _nextTurnIndex >= _turnOrder.Count)
                {
                    RebuildTurnOrder();
                }

                if (_turnOrder.Count == 0)
                {
                    return null;
                }

                while (_nextTurnIndex < _turnOrder.Count)
                {
                    var combatant = _turnOrder[_nextTurnIndex];
                    _nextTurnIndex++;

                    if (combatant.IsAlive)
                    {
                        return combatant;
                    }
                }
            }
        }

        public void RebuildTurnOrder()
        {
            _turnOrder.Clear();

            for (var index = 0; index < _combatants.Count; index++)
            {
                var combatant = _combatants[index];

                if (combatant != null && combatant.IsAlive)
                {
                    _turnOrder.Add(combatant);
                }
            }

            _turnOrder.Sort(CompareCombatants);
            _nextTurnIndex = 0;
        }

        private static int CompareCombatants(CombatantState left, CombatantState right)
        {
            var speedComparison = right.Speed.CompareTo(left.Speed);
            if (speedComparison != 0)
            {
                return speedComparison;
            }

            var sideComparison = GetSideSortOrder(left.Side).CompareTo(GetSideSortOrder(right.Side));
            if (sideComparison != 0)
            {
                return sideComparison;
            }

            return left.Rank.CompareTo(right.Rank);
        }

        private static int GetSideSortOrder(CombatSide side)
        {
            return side == CombatSide.Player ? 0 : 1;
        }
    }
}
