using System;
using System.Collections.Generic;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;

namespace DungeonCrawler.Combat
{
    public sealed class CombatController
    {
        private readonly IEventBus _eventBus;
        private readonly DamageResolver _damageResolver = new DamageResolver();
        private readonly TargetingRulesService _targetingRulesService = new TargetingRulesService();
        private TurnManager _turnManager;

        public CombatController(CombatFormationState formation, IEventBus eventBus)
        {
            Formation = formation ?? throw new ArgumentNullException(nameof(formation));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public CombatFormationState Formation { get; }

        public CombatState State { get; private set; } = CombatState.Initializing;

        public CombatantState CurrentCombatant { get; private set; }

        public IReadOnlyList<CombatantState> GetValidTargetsForCurrentCombatant(SkillDefinition skill)
        {
            return _targetingRulesService.GetValidTargets(skill, CurrentCombatant, Formation);
        }

        public TargetingValidationResult ValidateSkillTargetForCurrentCombatant(
            SkillDefinition skill,
            CombatantState target)
        {
            return _targetingRulesService.ValidateTarget(skill, CurrentCombatant, target);
        }

        public DamageResult ExecuteBasicAttack(CombatantState target)
        {
            if (State != CombatState.PlayerTurn && State != CombatState.EnemyTurn)
            {
                throw new InvalidOperationException("Basic attacks can only execute during an active combatant turn.");
            }

            if (CurrentCombatant == null)
            {
                throw new InvalidOperationException("There is no active combatant to execute a basic attack.");
            }

            var action = new CombatAction(CombatActionType.BasicAttack, CurrentCombatant, target);
            var result = _damageResolver.Resolve(action);
            _eventBus.Publish(new DamageResolvedEvent(result));
            PublishDeathEventIfNeeded(result);
            CompleteCurrentTurn();

            return result;
        }

        public DamageResult ExecuteSkill(SkillDefinition skill, CombatantState target)
        {
            if (skill == null)
            {
                throw new ArgumentNullException(nameof(skill));
            }

            if (State != CombatState.PlayerTurn && State != CombatState.EnemyTurn)
            {
                throw new InvalidOperationException("Skills can only execute during an active combatant turn.");
            }

            if (CurrentCombatant == null)
            {
                throw new InvalidOperationException("There is no active combatant to execute a skill.");
            }

            var validation = _targetingRulesService.ValidateTarget(skill, CurrentCombatant, target);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(validation.ErrorMessage);
            }

            var action = new CombatAction(
                CombatActionType.Skill, CurrentCombatant, target, skill.DamageMultiplier);
            var result = _damageResolver.Resolve(action);
            _eventBus.Publish(new DamageResolvedEvent(result));
            PublishDeathEventIfNeeded(result);
            CompleteCurrentTurn();

            return result;
        }

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

            if (resultState == CombatState.Victory)
            {
                _eventBus.Publish(new CombatVictoryEvent(Formation));
            }
            else if (resultState == CombatState.Defeat)
            {
                _eventBus.Publish(new CombatDefeatEvent(Formation));
            }

            _eventBus.Publish(new CombatEndedEvent(resultState));
        }

        private void PublishDeathEventIfNeeded(DamageResult result)
        {
            if (result.TargetHpBefore > 0 && result.TargetHpAfter == 0)
            {
                _eventBus.Publish(new CombatantDiedEvent(result.Target, result));
            }
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
