using System;
using System.Collections.Generic;
using DungeonCrawler.Combat;
using DungeonCrawler.Core.Services;

namespace DungeonCrawler.Core.Events
{
    [Serializable]
    public sealed class RunStartedEvent
    {
        public string RunId;
        public string Seed;
        public int CurrentFloor;
        public List<CombatantState> Party;

        public RunStartedEvent(string runId, string seed, int currentFloor, List<CombatantState> party)
        {
            RunId = runId;
            Seed = seed;
            CurrentFloor = currentFloor;
            Party = party;
        }
    }

    [Serializable]
    public sealed class RunAbandonedEvent
    {
        public string RunId;
        public DungeonRunState RunState;

        public RunAbandonedEvent(string runId, DungeonRunState runState)
        {
            RunId = runId;
            RunState = runState;
        }
    }

    [Serializable]
    public sealed class RunCompletedEvent
    {
        public string RunId;
        public DungeonRunState RunState;
        public Dictionary<string, object> Rewards;

        public RunCompletedEvent(string runId, DungeonRunState runState, Dictionary<string, object> rewards)
        {
            RunId = runId;
            RunState = runState;
            Rewards = rewards;
        }
    }

    [Serializable]
    public sealed class FloorAdvancedEvent
    {
        public string RunId;
        public int CurrentFloor;
        public string CurrentThemeId;

        public FloorAdvancedEvent(string runId, int currentFloor, string currentThemeId)
        {
            RunId = runId;
            CurrentFloor = currentFloor;
            CurrentThemeId = currentThemeId;
        }
    }

    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Publish<TEvent>(TEvent gameEvent)
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                ((Action<TEvent>)handlers)?.Invoke(gameEvent);
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var eventType = typeof(TEvent);
            _handlers[eventType] = _handlers.TryGetValue(eventType, out var existing)
                ? Delegate.Combine(existing, handler)
                : handler;
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var eventType = typeof(TEvent);
            if (!_handlers.TryGetValue(eventType, out var existing))
            {
                return;
            }

            var updated = Delegate.Remove(existing, handler);
            if (updated == null)
            {
                _handlers.Remove(eventType);
                return;
            }

            _handlers[eventType] = updated;
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}
