namespace DungeonCrawler.Combat
{
    public readonly struct CombatStartedEvent
    {
        public CombatStartedEvent(CombatFormationState formation)
        {
            Formation = formation;
        }

        public CombatFormationState Formation { get; }
    }

    public readonly struct CombatStateChangedEvent
    {
        public CombatStateChangedEvent(CombatState previousState, CombatState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public CombatState PreviousState { get; }

        public CombatState CurrentState { get; }
    }

    public readonly struct TurnStartedEvent
    {
        public TurnStartedEvent(CombatantState combatant, CombatState state)
        {
            Combatant = combatant;
            State = state;
        }

        public CombatantState Combatant { get; }

        public CombatState State { get; }
    }

    public readonly struct TurnEndedEvent
    {
        public TurnEndedEvent(CombatantState combatant)
        {
            Combatant = combatant;
        }

        public CombatantState Combatant { get; }
    }

    public readonly struct CombatEndedEvent
    {
        public CombatEndedEvent(CombatState resultState)
        {
            ResultState = resultState;
        }

        public CombatState ResultState { get; }
    }

    public readonly struct CombatantDiedEvent
    {
        public CombatantDiedEvent(CombatantState combatant, DamageResult damageResult)
        {
            Combatant = combatant;
            DamageResult = damageResult;
        }

        public CombatantState Combatant { get; }

        public DamageResult DamageResult { get; }
    }

    public readonly struct CombatVictoryEvent
    {
        public CombatVictoryEvent(CombatFormationState formation)
        {
            Formation = formation;
        }

        public CombatFormationState Formation { get; }
    }

    public readonly struct CombatDefeatEvent
    {
        public CombatDefeatEvent(CombatFormationState formation)
        {
            Formation = formation;
        }

        public CombatFormationState Formation { get; }
    }

    public readonly struct DamageResolvedEvent
    {
        public DamageResolvedEvent(DamageResult result)
        {
            Result = result;
        }

        public DamageResult Result { get; }
    }
}
