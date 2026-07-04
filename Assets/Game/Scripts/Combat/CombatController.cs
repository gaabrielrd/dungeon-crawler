using System;
using DungeonCrawler.Core.Services;

namespace DungeonCrawler.Combat
{
    public sealed class CombatController
    {
        private readonly IEventBus _eventBus;
        private TurnManager _turnManager;

        public CombatController(CombatFormationState formation, IEventBus eventBus)
        {
            Formation = formation ?? throw new ArgumentNullException(nameof(formation));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public CombatFormationState Formation { get; }

        public CombatState State { get; private set; } = CombatState.Initializing;

        public CombatantState CurrentCombatant { get; private set; }

        public void StartCombat()
        {
            if (State != CombatState.Initializing)
            {
                throw new InvalidOperationException("Combat has already started.");
            }

            ValidateFormation();

            _turnManager = new TurnManager(Formation);
            _eventBus.Publish(new CombatStartedEvent(Formation));

            StartNextTurn();
        }

        public void CompleteCurrentTurn()
        {
            if (State == CombatState.Initializing)
            {
                throw new InvalidOperationException("Combat has not started.");
            }

            if (State == CombatState.Victory || State == CombatState.Defeat)
            {
                throw new InvalidOperationException("Combat has already ended.");
            }

            if (CurrentCombatant == null)
            {
                throw new InvalidOperationException("There is no active combatant turn to complete.");
            }

            _eventBus.Publish(new TurnEndedEvent(CurrentCombatant));
            ChangeState(CombatState.Resolving);

            if (TryEndCombat())
            {
                return;
            }

            StartNextTurn();
        }

        private void StartNextTurn()
        {
            if (TryEndCombat())
            {
                return;
            }

            CurrentCombatant = _turnManager.GetNextCombatant();
            if (CurrentCombatant == null)
            {
                throw new InvalidOperationException("Combat cannot continue without living combatants.");
            }

            ChangeState(CurrentCombatant.Side == CombatSide.Player
                ? CombatState.PlayerTurn
                : CombatState.EnemyTurn);

            _eventBus.Publish(new TurnStartedEvent(CurrentCombatant, State));
        }

        private bool TryEndCombat()
        {
            if (!HasLivingCombatants(CombatSide.Enemy))
            {
                EndCombat(CombatState.Victory);
                return true;
            }

            if (!HasLivingCombatants(CombatSide.Player))
            {
                EndCombat(CombatState.Defeat);
                return true;
            }

            return false;
        }

        private void EndCombat(CombatState resultState)
        {
            CurrentCombatant = null;
            ChangeState(resultState);
            _eventBus.Publish(new CombatEndedEvent(resultState));
        }

        private void ChangeState(CombatState nextState)
        {
            if (State == nextState)
            {
                return;
            }

            var previousState = State;
            State = nextState;
            _eventBus.Publish(new CombatStateChangedEvent(previousState, State));
        }

        private void ValidateFormation()
        {
            var playerCount = Formation.CountSide(CombatSide.Player);
            var enemyCount = Formation.CountSide(CombatSide.Enemy);

            if (playerCount <= 0)
            {
                throw new InvalidOperationException("Combat requires at least one player combatant.");
            }

            if (enemyCount <= 0)
            {
                throw new InvalidOperationException("Combat requires at least one enemy combatant.");
            }

            if (playerCount > CombatRank.MaxCombatantsPerSide || enemyCount > CombatRank.MaxCombatantsPerSide)
            {
                throw new InvalidOperationException(
                    $"Combat cannot contain more than {CombatRank.MaxCombatantsPerSide} combatants per side.");
            }

            if (!HasLivingCombatants(CombatSide.Player))
            {
                throw new InvalidOperationException("Combat requires at least one living player combatant.");
            }

            if (!HasLivingCombatants(CombatSide.Enemy))
            {
                throw new InvalidOperationException("Combat requires at least one living enemy combatant.");
            }
        }

        private bool HasLivingCombatants(CombatSide side)
        {
            var combatants = Formation.Combatants;

            for (var index = 0; index < combatants.Count; index++)
            {
                var combatant = combatants[index];

                if (combatant.Side == side && combatant.IsAlive)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
