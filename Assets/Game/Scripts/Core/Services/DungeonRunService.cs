using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Events;
using DungeonCrawler.Core.Services;
using DungeonCrawler.Data.Definitions;
using DungeonCrawler.Dungeon;
using DungeonCrawler.Economy;

namespace DungeonCrawler.Core.Services
{
    public sealed class DungeonRunService : IDungeonRunService
    {
        private readonly IEventBus _eventBus;
        private readonly RewardResolver _rewardResolver = new RewardResolver();

        public DungeonRunService(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventBus.Subscribe<CombatEndedEvent>(OnCombatEnded);
        }

        public bool IsInitialized { get; private set; }

        public bool HasActiveRun { get; private set; }

        public DungeonRunState ActiveRun { get; private set; }

        public CombatController CurrentCombatController { get; private set; }

        public DungeonThemeDefinition CurrentThemeDefinition { get; set; }

        public RewardDefinition CurrentRewardDefinition { get; set; }

        public DungeonThemeResolver ThemeResolver { get; set; }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
            IsInitialized = true;
        }

        public async Task<DungeonRunState> StartRunAsync(string seed = null, List<CombatantState> party = null)
        {
            if (HasActiveRun)
            {
                throw new InvalidOperationException("Run already in progress.");
            }

            var run = DungeonRunState.CreateNew(seed, party);
            ActiveRun = run;
            HasActiveRun = true;

            if (ThemeResolver != null)
            {
                var theme = ThemeResolver.GetTheme(1);
                CurrentThemeDefinition = theme;
                run.CurrentThemeId = theme.Id;
            }

            GenerateCurrentFloor();
            ActiveRun.Status = DungeonRunStatus.Exploring;

            _eventBus.Publish(new RunStartedEvent(run.RunId, run.Seed, run.CurrentFloor, run.Party));

            return run;
        }

        public async Task AbandonRunAsync(string runId)
        {
            if (!HasActiveRun || ActiveRun.RunId != runId)
            {
                throw new InvalidOperationException("No active run or run not found.");
            }

            HasActiveRun = false;
            var abandonedRun = ActiveRun;
            ActiveRun = null;
            CurrentCombatController = null;

            _eventBus.Publish(new RunAbandonedEvent(runId, abandonedRun));
        }

        public async Task CompleteRunAsync(string runId, Dictionary<string, object> rewards)
        {
            if (!HasActiveRun || ActiveRun.RunId != runId)
            {
                throw new InvalidOperationException("No active run or run not found.");
            }

            var completedRun = ActiveRun;
            completedRun.inventorySnapshot = new SaveProfileSnapshot();
            completedRun.visitedFloors.Add(completedRun.CurrentFloor);
            completedRun.status = DungeonRunStatus.Completed;
            completedRun.canAdvanceFloor = false;

            HasActiveRun = false;
            CurrentCombatController = null;
            _eventBus.Publish(new RunCompletedEvent(runId, completedRun, rewards));
        }

        public async Task<DungeonRunState> LoadRunAsync(string runId)
        {
            var seed = "test-seed-123";
            var generator = new FloorGenerator();
            var floor = generator.GenerateFloor(seed, 5, "crypt");

            var run = new DungeonRunState
            {
                runId = runId,
                seed = seed,
                currentFloor = 5,
                currentThemeId = floor.ThemeId,
                currentFloorInfo = floor,
                party = new List<CombatantState>(),
                inventorySnapshot = new SaveProfileSnapshot(),
                visitedFloors = new HashSet<int> { 1, 2, 3, 4, 5 },
                startedAtUtc = DateTime.UtcNow.AddDays(-2).ToString("O"),
                status = DungeonRunStatus.Exploring,
                lastCombatResult = CombatState.Initializing,
                canAdvanceFloor = false,
                lastResolvedReward = null
            };

            return run;
        }

        public GeneratedFloor GenerateCurrentFloor()
        {
            if (!HasActiveRun)
            {
                throw new InvalidOperationException("No active run to generate a floor for.");
            }

            if (ThemeResolver != null)
            {
                var theme = ThemeResolver.GetTheme(ActiveRun.CurrentFloor);
                if (theme != CurrentThemeDefinition)
                {
                    CurrentThemeDefinition = theme;
                    ActiveRun.CurrentThemeId = theme.Id;
                }
            }

            var generated = GenerateFloor(ActiveRun.CurrentFloor, ActiveRun.CurrentThemeId);
            ActiveRun.ApplyFloorGeneration(generated);
            return generated;
        }

        public CombatController StartCurrentFloorCombat(CombatFormationState formation)
        {
            if (!HasActiveRun)
            {
                throw new InvalidOperationException("No active run to start combat for.");
            }

            if (formation == null)
            {
                throw new ArgumentNullException(nameof(formation));
            }

            if (ActiveRun.Status == DungeonRunStatus.Failed)
            {
                throw new InvalidOperationException("Failed runs cannot start combat.");
            }

            if (ActiveRun.Status == DungeonRunStatus.FloorResolved)
            {
                throw new InvalidOperationException("Current floor is already resolved.");
            }

            if (ActiveRun.CurrentFloorInfo == null)
            {
                GenerateCurrentFloor();
            }

            if (ActiveRun.CurrentFloorInfo.PrimaryType != FloorType.Combat
                && ActiveRun.CurrentFloorInfo.PrimaryType != FloorType.Boss)
            {
                throw new InvalidOperationException(
                    $"Floor type {ActiveRun.CurrentFloorInfo.PrimaryType} cannot start combat.");
            }

            ActiveRun.Status = DungeonRunStatus.InCombat;
            ActiveRun.CanAdvanceFloor = false;
            ActiveRun.LastCombatResult = CombatState.Initializing;

            CurrentCombatController = new CombatController(formation, _eventBus);
            CurrentCombatController.StartCombat();
            return CurrentCombatController;
        }

        public void ResolveCurrentCombatResult(CombatState result)
        {
            if (!HasActiveRun)
            {
                throw new InvalidOperationException("No active run to resolve combat for.");
            }

            if (result != CombatState.Victory && result != CombatState.Defeat)
            {
                throw new ArgumentException("Only victory or defeat can resolve a combat result.", nameof(result));
            }

            if (ActiveRun.Status == DungeonRunStatus.FloorResolved && result == CombatState.Victory)
            {
                return;
            }

            if (ActiveRun.Status == DungeonRunStatus.Failed && result == CombatState.Defeat)
            {
                return;
            }

            if (ActiveRun.Status != DungeonRunStatus.InCombat)
            {
                throw new InvalidOperationException("Combat results can only be resolved while the run is in combat.");
            }

            ActiveRun.LastCombatResult = result;
            CurrentCombatController = null;

            if (result == CombatState.Victory)
            {
                ResolveAndApplyVictoryReward();
                ActiveRun.VisitedFloors.Add(ActiveRun.CurrentFloor);
                ActiveRun.Status = DungeonRunStatus.FloorResolved;
                ActiveRun.CanAdvanceFloor = true;
                return;
            }

            ActiveRun.Status = DungeonRunStatus.Failed;
            ActiveRun.CanAdvanceFloor = false;
        }

        public void AdvanceFloor()
        {
            if (!HasActiveRun)
            {
                throw new InvalidOperationException("No active run to advance.");
            }

            if (ActiveRun.Status != DungeonRunStatus.FloorResolved || !ActiveRun.CanAdvanceFloor)
            {
                throw new InvalidOperationException("Current floor must be resolved before advancing.");
            }

            var nextFloor = ActiveRun.CurrentFloor + 1;

            if (ThemeResolver != null && ThemeResolver.IsThemeTransitionFloor(nextFloor))
            {
                var nextTheme = ThemeResolver.GetTheme(nextFloor);
                CurrentThemeDefinition = nextTheme;
                ActiveRun.CurrentThemeId = nextTheme.Id;
            }

            var generated = GenerateFloor(nextFloor, ActiveRun.CurrentThemeId);

            ActiveRun.AdvanceFloor();
            ActiveRun.ApplyFloorGeneration(generated);
            ActiveRun.Status = DungeonRunStatus.Exploring;
            ActiveRun.LastCombatResult = CombatState.Initializing;
            ActiveRun.CanAdvanceFloor = false;
            ActiveRun.LastResolvedReward = null;
            CurrentCombatController = null;

            _eventBus.Publish(new FloorAdvancedEvent(ActiveRun.RunId, ActiveRun.CurrentFloor, ActiveRun.CurrentThemeId));
        }

        public void AbandonCurrentRun()
        {
            if (HasActiveRun)
            {
                var runId = ActiveRun.RunId;
                HasActiveRun = false;
                _eventBus.Publish(new RunAbandonedEvent(runId, ActiveRun));
                ActiveRun = null;
                CurrentCombatController = null;
            }
        }

        public void CompleteCurrentRun(Dictionary<string, object> rewards)
        {
            if (HasActiveRun)
            {
                var runId = ActiveRun.RunId;
                ActiveRun.Status = DungeonRunStatus.Completed;
                ActiveRun.CanAdvanceFloor = false;
                HasActiveRun = false;
                _eventBus.Publish(new RunCompletedEvent(runId, ActiveRun, rewards));
                ActiveRun = null;
                CurrentCombatController = null;
            }
        }

        private GeneratedFloor GenerateFloor(int floorNumber, string themeId)
        {
            var floorGen = new FloorGenerator();
            string nextThemeOverride = ThemeResolver?.GetNextThemeId(floorNumber);
            var generated = floorGen.GenerateFloor(ActiveRun.Seed, floorNumber, themeId, nextThemeOverride);

            if (ThemeResolver != null)
            {
                var correctThemeId = ThemeResolver.GetThemeId(floorNumber);
                if (generated.ThemeId != correctThemeId)
                {
                    generated = new GeneratedFloor(
                        generated.FloorNumber,
                        generated.PrimaryType,
                        generated.HasRestingSite,
                        generated.IsThemeTransition,
                        correctThemeId,
                        generated.NextThemeId,
                        generated.LocalSeed,
                        generated.Encounter
                    );
                }
            }

            if (CurrentThemeDefinition != null)
            {
                var encounterGen = new EncounterGenerator();
                var encounter = encounterGen.Resolve(CurrentThemeDefinition, generated.PrimaryType, generated.LocalSeed);
                generated = generated.WithEncounter(encounter);
            }

            return generated;
        }

        private void ResolveAndApplyVictoryReward()
        {
            if (ActiveRun.LastResolvedReward != null)
            {
                return;
            }

            if (ActiveRun.InventorySnapshot == null)
            {
                ActiveRun.InventorySnapshot = new SaveProfileSnapshot();
            }

            ActiveRun.InventorySnapshot.Normalize();
            var context = CreateRewardContext();
            var reward = _rewardResolver.Resolve(context, CurrentRewardDefinition);

            ActiveRun.InventorySnapshot.SoftCurrency += reward.SoftCurrency;
            for (var index = 0; index < reward.ItemRewards.Count; index++)
            {
                var item = reward.ItemRewards[index];
                ActiveRun.InventorySnapshot.AddItem(item.ItemId, item.Quantity);
            }

            SyncCombatPartyToRoster();
            ApplyXpToHeroes(reward.XpReward);
            RebuildCombatPartyFromRoster();

            ActiveRun.LastResolvedReward = reward;
            ApplyRewardToSave(reward);
        }

        private void SyncCombatPartyToRoster()
        {
            var roster = ActiveRun.Roster;
            var party = ActiveRun.Party;
            if (roster == null || party == null)
            {
                return;
            }

            for (var i = 0; i < roster.Count; i++)
            {
                var hero = roster[i];
                if (hero == null)
                {
                    continue;
                }

                var combatant = FindPartyCombatant(hero);
                if (combatant != null)
                {
                    hero.SyncAfterCombat(combatant);
                }
            }
        }

        private void RebuildCombatPartyFromRoster()
        {
            var roster = ActiveRun.Roster;
            if (roster == null || roster.Count == 0)
            {
                return;
            }

            var party = new List<CombatantState>();
            for (var i = 0; i < roster.Count; i++)
            {
                var hero = roster[i];
                if (hero == null || !hero.IsInParty)
                {
                    continue;
                }

                var rank = hero.PartyRank > 0 ? hero.PartyRank : party.Count + 1;
                party.Add(CombatantStateFactory.CreateHeroFromState(hero, rank));
            }

            ActiveRun.Party = party;
        }

        private CombatantState FindPartyCombatant(DungeonCrawler.Data.State.HeroState hero)
        {
            var party = ActiveRun.Party;
            for (var i = 0; i < party.Count; i++)
            {
                var combatant = party[i];
                if (combatant != null
                    && combatant.DefinitionId == hero.ClassId
                    && combatant.Rank == hero.PartyRank)
                {
                    return combatant;
                }
            }

            return null;
        }

        private void ApplyXpToHeroes(int xpAmount)
        {
            if (xpAmount <= 0)
                return;

            var roster = ActiveRun.Roster;
            if (roster == null || roster.Count == 0)
                return;

            for (var i = 0; i < roster.Count; i++)
            {
                var hero = roster[i];
                if (hero == null || hero.IsDead)
                    continue;

                hero.AddXp(xpAmount);

                while (HeroProgressionService.TryLevelUp(hero))
                {
                }
            }
        }

        private RewardContext CreateRewardContext()
        {
            var floor = ActiveRun.CurrentFloorInfo;
            var floorType = floor?.PrimaryType ?? FloorType.Combat;
            var encounterType = floor?.Encounter?.EncounterType ?? MapEncounterType(floorType);
            var localSeed = floor?.LocalSeed ?? 0;

            return new RewardContext(
                ActiveRun.Seed,
                ActiveRun.CurrentFloor,
                localSeed,
                floorType,
                encounterType);
        }

        private static EncounterType MapEncounterType(FloorType floorType)
        {
            return floorType == FloorType.Boss ? EncounterType.Boss : EncounterType.Common;
        }

        private static void ApplyRewardToSave(ResolvedReward reward)
        {
            if (reward == null || reward.SoftCurrency <= 0)
            {
                return;
            }

            if (!ServiceRegistry.TryResolve<ISaveService>(out var saveService)
                || saveService.Current == null
                || saveService.Current.Profile == null)
            {
                return;
            }

            saveService.Current.Profile.SoftCurrency += reward.SoftCurrency;
            saveService.SaveAsync(saveService.Current).GetAwaiter().GetResult();
        }

        private void OnCombatEnded(CombatEndedEvent gameEvent)
        {
            if (!HasActiveRun || ActiveRun.Status != DungeonRunStatus.InCombat)
            {
                return;
            }

            ResolveCurrentCombatResult(gameEvent.ResultState);
        }
    }
}
